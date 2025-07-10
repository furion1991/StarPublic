using DtoClassLibrary.DataTransferObjects.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Threading.Channels;
using DtoClassLibrary.DataTransferObjects.Cache;

namespace DataTransferLib.CacheServices;

public class RabbitMqCacheService(ILogger<RabbitMqCacheService> logger)
{
    private IConnection? _connection;
    private readonly ConnectionFactory _factory = new()
    {
        HostName = "rabbitmq",
        UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? string.Empty,
        Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? string.Empty
    };

    public async Task SendCasesItemsReloadMessage()
    {
        var envelope = new CacheExchangeEnvelope()
        {
            Payload = "reload",
            Type = "casesitemsreload"
        };
        var message = JsonConvert.SerializeObject(envelope);

        await SendCacheMessage(message, envelope.Type);
    }



    public async Task SendReloadUserCache(string id)
    {
        var envelope = new CacheExchangeEnvelope()
        {
            Payload = id,
            Type = "userreload"
        };
        var message = JsonConvert.SerializeObject(envelope);
        await SendCacheMessage(message, envelope.Type);
    }

    public async Task SendRebootCacheMessage()
    {
        var envelope = new CacheExchangeEnvelope()
        {
            Payload = "reboot",
            Type = "warmup"
        };
        var message = JsonConvert.SerializeObject(envelope);
        await SendCacheMessage(message, envelope.Type);
    }
    public async Task SendCacheMessage<T>(T entity)
    {
        var envelope = new CacheExchangeEnvelope()
        {
            Payload = JsonConvert.SerializeObject(entity),
            Type = typeof(T).Name.ToLower()
        };
        var message = JsonConvert.SerializeObject(envelope);

        await SendCacheMessage(message, envelope.Type);
    }


    private async Task EstablishConnectionWithRetry(CancellationToken cancellationToken, int maxRetries = 20,
        int delayMs = 2000)
    {
        var attempts = 0;
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
    private async Task SendCacheMessage(string message, string routingKey)
    {
        if (_connection is not { IsOpen: true })
        {
            await EstablishConnectionWithRetry(CancellationToken.None);
        }

        await using var channel = await _connection!.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConfig.CacheExchangeName,
            type: RabbitMqConfig.ExchangeType,
            durable: RabbitMqConfig.Durable,
            autoDelete: RabbitMqConfig.AutoDelete
        );

        var queue = await channel.QueueDeclareAsync(
            queue: RabbitMqConfig.CacheQueueName,
            durable: RabbitMqConfig.Durable,
            exclusive: RabbitMqConfig.Exclusive,
            autoDelete: RabbitMqConfig.AutoDelete
        );

        await channel.QueueBindAsync(queue: queue.QueueName, exchange: RabbitMqConfig.CacheExchangeName, routingKey: routingKey.ToLower());

        var body = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties
        {
            Persistent = RabbitMqConfig.Persistent
        };

        await channel.BasicPublishAsync(RabbitMqConfig.CacheExchangeName, routingKey.ToLower(), body: body);
    }
}