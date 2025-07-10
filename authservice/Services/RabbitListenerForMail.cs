using System.Text;
using AuthService.Database.Context;
using DtoClassLibrary.DataTransferObjects.Auth;
using DtoClassLibrary.DataTransferObjects.Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace AuthService.Services
{
    public class RabbitListenerForMail(IServiceProvider sp,
        IDbContextFactory<AuthContext> factory,
        ILogger<RabbitListenerForMail> logger) : BackgroundService
    {
        private IConnection _connection;

        private IChannel _channel;
        private readonly ConnectionFactory _connectionFactory = new()
        {
            HostName = "rabbitmq",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? string.Empty,
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? string.Empty
        };
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if (_connection == null || _connection.IsOpen == false)
            {
                await EstablishConnectionWithRetry(stoppingToken);
            }

            if (_connection == null || !_connection.IsOpen)
            {
                throw new BrokerUnreachableException(new ArgumentException("failed to connect to rabbitmq"));
            }

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
            var confirmationQueue = await _channel.QueueDeclareAsync(RabbitMqConfig.EmailConfirmationQueueName, true, false, false,
                cancellationToken: stoppingToken);
            await _channel.ExchangeDeclareAsync(exchange: RabbitMqConfig.EmailExchangeName, RabbitMqConfig.ExchangeType,
                true, false, cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(confirmationQueue.QueueName, RabbitMqConfig.EmailExchangeName,
                RabbitMqConfig.EmailConfirmationRoutingKey, cancellationToken: stoppingToken);

            var confirmationMessageConsumer = new AsyncEventingBasicConsumer(_channel);
            confirmationMessageConsumer.ReceivedAsync += HandleConfirmationMessage;
            await _channel.BasicConsumeAsync(confirmationQueue.QueueName, false, confirmationMessageConsumer, stoppingToken);

            var resetQueue = await _channel.QueueDeclareAsync(RabbitMqConfig.PasswordResetQueueName, true, false, false,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(resetQueue.QueueName, RabbitMqConfig.EmailExchangeName,
                RabbitMqConfig.PasswordResetRoutingKey, cancellationToken: stoppingToken);
            var resetConsumer = new AsyncEventingBasicConsumer(_channel);

            resetConsumer.ReceivedAsync += HandlePasswordResetMessage;
            await _channel.BasicConsumeAsync(resetQueue.QueueName, false, resetConsumer, stoppingToken);


            await Task.CompletedTask;
        }



        private async Task HandlePasswordResetMessage(object sender, BasicDeliverEventArgs ea)
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            logger.LogInformation($"Received message from {sender} service, \nThe message is: {message}");
            if (ea.RoutingKey == RabbitMqConfig.PasswordResetRoutingKey)
            {
                await using var scope = sp.CreateAsyncScope();
                var resetMessage = JsonConvert.DeserializeObject<ResetPasswordEmailMessage>(message);
                var emailSender = sp.GetRequiredService<EmailSenderService>();

                if (resetMessage != null)
                {
                    var sb = new StringBuilder();
                    sb.Append("Ваш новый пароль:\n");
                    sb.Append(resetMessage.NewPassword);
                    await emailSender.SendEmailAsync(resetMessage.To, "Reset email", sb.ToString());
                }
            }
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }


        private async Task HandleConfirmationMessage(object sender, BasicDeliverEventArgs ea)
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            logger.LogInformation($"Received message from {sender} service, \nThe message is: {message}");

            if (ea.RoutingKey == RabbitMqConfig.EmailConfirmationRoutingKey)
            {
                await using var scope = sp.CreateAsyncScope();
                var confirmMessage = JsonConvert.DeserializeObject<ConfirmationEmailMessage>(message);
                var emailSender = sp.GetRequiredService<EmailSenderService>();
                if (confirmMessage != null)
                {
                    var messageGen = emailSender.GenerateConfirmationMessage(confirmMessage.ConfirmationLink);
                    await emailSender.SendEmailAsync(confirmMessage.To, "Confirmation email", messageGen);
                }
            }


            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }

        private async Task EstablishConnectionWithRetry(CancellationToken cancellationToken, int maxRetries = 20,
            int delayMs = 3000)
        {
            int attempts = 0;
            while (attempts < maxRetries && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
                    logger.LogInformation("Connection with RabbitMq established");
                    break;
                }
                catch (BrokerUnreachableException)
                {
                    logger.LogWarning($"Connection to RabbitMQ failed, attempt: {attempts}");
                    attempts++;
                    await Task.Delay(delayMs, cancellationToken);
                }
            }

            if (_connection == null || !_connection.IsOpen)
            {
                logger.LogError($"Failed to establish connection to RabbitMQ");
            }
        }
    }
}
