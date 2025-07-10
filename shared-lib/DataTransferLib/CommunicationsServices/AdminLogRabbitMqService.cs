using System.Text;
using DtoClassLibrary.DataTransferObjects.Common.Logs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace DataTransferLib.CommunicationsServices;

public class AdminLogRabbitMqService(ILogger<AdminLogRabbitMqService> logger) : IDisposable
{
    private IConnection? _connection;
    private readonly ConnectionFactory _factory = new()
    {
        HostName = "rabbitmq",
        UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? string.Empty,
        Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? string.Empty
    };

    private const string ExchangeName = "admin.logs.exchange";


    public async Task SendAdminLog<T>(AdminLog<T> log) where T : class
    {
        var queueName = $"admin.log.{typeof(T).Name.ToLower()}";
        var routingKey = queueName;


        var envelope = new
        {
            Type = typeof(T).Name,
            Payload = log.Payload,
            Message = log.Message,
            ActionPerformedBy = log.ActionPerformedBy
        };

        var json = JsonConvert.SerializeObject(envelope);

        if (_connection == null || !_connection.IsOpen)
        {
            await EstablishConnectionWithRetry(CancellationToken.None);
        }

        await using var channel = await _connection!.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: "topic",
            durable: true,
            autoDelete: false);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: queueName,
            exchange: ExchangeName,
            routingKey: routingKey);

        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            body: body);

        logger.LogInformation($"[AdminLog] Сообщение отправлено в очередь {queueName}.");
    }



    private async Task EstablishConnectionWithRetry(CancellationToken cancellationToken, int maxRetries = 10, int delayMs = 2000)
    {
        int attempts = 0;
        while (attempts < maxRetries && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                logger.LogInformation("Соединение с RabbitMQ (AdminLog) установлено.");
                break;
            }
            catch (BrokerUnreachableException)
            {
                logger.LogWarning($"[AdminLog] Попытка {attempts} подключения к RabbitMQ не удалась. Повтор через {delayMs} мс...");
                attempts++;
                await Task.Delay(delayMs, cancellationToken);
            }
        }

        if (_connection == null)
        {
            logger.LogError("[AdminLog] Не удалось установить соединение с RabbitMQ после {maxRetries} попыток.");
        }
    }
    public void Dispose()
    {
        _connection?.CloseAsync();
    }
}