using System.Text;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Auth;
using DtoClassLibrary.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DataTransferLib.CommunicationsServices;

/// <summary>Сервис для обслуживания работы с логами</summary>
public class RabbitMqService(ILogger<RabbitMqService> logger) : IService, IDisposable
{
    private IConnection? _connection;
    private readonly ConnectionFactory _factory = new()
    {
        HostName = "rabbitmq",
        UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? string.Empty,
        Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? string.Empty
    };

    public async Task SendLog(string message, object content, LTYPE logType, string? performedById = null,
        string? objectId = null, int? type = null)
    {
        Log log = new()
        {
            Message = message,
            Content = content,
            LogType = logType,
            PerformedById = performedById,
            ObjectId = objectId,
            Type = type
        };

        await SendLog(log);
    }

    public async Task SendEmailConfirmationMessage(ConfirmationEmailMessage message)
    {
        var json = JsonConvert.SerializeObject(message);

        await SendMessage(json, RabbitMqConfig.EmailConfirmationQueueName, RabbitMqConfig.EmailExchangeName, RabbitMqConfig.EmailConfirmationRoutingKey);
    }

    public async Task SendResetPasswordMessage(ResetPasswordEmailMessage message)
    {
        var json = JsonConvert.SerializeObject(message);
        await SendMessage(json, RabbitMqConfig.PasswordResetQueueName, RabbitMqConfig.EmailExchangeName,
            RabbitMqConfig.PasswordResetRoutingKey);
    }

    private async Task SendMessage(string message, string queueName, string exchangeName = "", string routingKey = "")
    {
        if (_connection == null || !_connection.IsOpen)
        {
            await EstablishConnectionWithRetry(CancellationToken.None);
        }
        await using var channel = await _connection!.CreateChannelAsync();

        var body = Encoding.UTF8.GetBytes(message).AsMemory();

        var properties = new BasicProperties()
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(exchange: exchangeName, routingKey: routingKey, body: body);
    }


    public async Task SendLog(Log log)
    {
        await SendLogMessage(JsonSerializer.Serialize(log), $"log_{log.LogType}");
    }

    public async Task SendLogMessage(string message, string queueName)
    {
        if (_connection == null || !_connection.IsOpen)
        {
            await EstablishConnectionWithRetry(CancellationToken.None);
        }

        await using var channel = await _connection!.CreateChannelAsync();

        // Объявляем exchange, если он не объявлен
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConfig.LogsExchangeName,
            type: RabbitMqConfig.ExchangeType,
            durable: RabbitMqConfig.Durable,
            autoDelete: RabbitMqConfig.AutoDelete
        );

        // Объявляем очередь
        var queue = await channel.QueueDeclareAsync(
            queue: queueName,
            durable: RabbitMqConfig.Durable,
            exclusive: RabbitMqConfig.Exclusive,
            autoDelete: RabbitMqConfig.AutoDelete
        );

        Log? log = JsonSerializer.Deserialize<Log>(message);
        LTYPE logType = log?.LogType ?? LTYPE.Base;

        // Привязываем очередь к exchange с корректным routingKey
        await channel.QueueBindAsync(queue: queue.QueueName, exchange: RabbitMqConfig.LogsExchangeName, routingKey: queueName);

        var body = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties
        {
            Persistent = RabbitMqConfig.Persistent
        };

        // Используем queueName в качестве routingKey
        await channel.BasicPublishAsync(RabbitMqConfig.LogsExchangeName, queueName, body: body);

        logger.LogInformation($"Лог {message} отправлен в очередь {queue.QueueName}");
    }



    private async Task EstablishConnectionWithRetry(CancellationToken cancellationToken, int maxRetries = 20,
        int delayMs = 2000)
    {
        int attempts = 0;
        while (attempts < maxRetries && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                logger.LogInformation("Соединение с RabbitMQ установлено.");
                break;
            }
            catch (BrokerUnreachableException)
            {
                logger.LogWarning(
                    $"Попытка {attempts} подключения к RabbitMQ не удалась. Повтор через {delayMs} мс...");
                attempts++;
                await Task.Delay(delayMs, cancellationToken);
            }
        }

        if (_connection == null)
        {
            logger.LogError("Не удалось установить соединение с RabbitMQ после максимального количества попыток.");
        }
    }

    public void Dispose()
    {
        _connection.CloseAsync();
    }
}