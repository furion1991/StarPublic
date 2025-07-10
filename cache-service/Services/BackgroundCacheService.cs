using System.Text;
using DataTransferLib.CacheServices;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Cache;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.Users;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace cache_service.Services;

public class BackgroundCacheService(
    ILogger<BackgroundCacheService> logger,
    IServiceProvider serviceProvider,
    CasesCommService casesCommService) : BackgroundService
{
    public static readonly List<string> routingKeys = ["itemdto", "casedto", "userdto", "warmup", "casesitemsreload", "usersreload", "userreload"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await HandleRabbitMqCacheConnectionsAsync(stoppingToken);
        await WarmupCache();
    }

    public async Task CasesItemsCacheReload()
    {
        await ReloadItemsCache();
        await ReloadCasesCache();
    }





    private async Task HandleRabbitMqCacheConnectionsAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? string.Empty,
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? string.Empty
        };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync("cache_exchange", ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);

        var queue = await channel.QueueDeclareAsync("cache_queue", durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

        foreach (var routingKey in routingKeys)
        {
            await channel.QueueBindAsync(queue: queue.QueueName, exchange: "cache_exchange", routingKey: routingKey, cancellationToken: stoppingToken);
        }
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var envelope = JsonConvert.DeserializeObject<CacheExchangeEnvelope>(message);
            if (envelope is null)
            {
                return;
            }
            await UnwrapEnvelope(envelope);

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        };

        await channel.BasicConsumeAsync(queue: queue.QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }

    private async Task UnwrapEnvelope(CacheExchangeEnvelope envelope)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();

            switch (envelope.Type.ToLower())
            {
                case "itemdto":
                    var itemsCache = scope.ServiceProvider.GetRequiredService<ItemsCache>();
                    var item = JsonConvert.DeserializeObject<ItemDto>(envelope.Payload);
                    await ReloadItemsCache();
                    await ReloadCasesCache();
                    await itemsCache.SetItemToCache(item);
                    break;
                case "casedto":
                    var casesCache = scope.ServiceProvider.GetRequiredService<CasesCache>();
                    var c = JsonConvert.DeserializeObject<CaseDto>(envelope.Payload);
                    await ReloadCasesCache();
                    await ReloadItemsCache();
                    await casesCache.SetCaseToCache(c);
                    break;
                case "userdto":
                    var usersCache = scope.ServiceProvider.GetRequiredService<UsersCache>();
                    var u = JsonConvert.DeserializeObject<UserDto>(envelope.Payload);
                    await usersCache.SetUserToCache(u);
                    break;
                case "warmup":
                    await WarmupCache();
                    break;
                case "casesitemsreload":
                    await CasesItemsCacheReload();
                    break;
                case "usersreload":
                    await ReloadUsersCache();
                    break;
                case "userreload":
                    var userId = JsonConvert.DeserializeObject<string>(envelope.Payload);
                    if (userId != null)
                    {
                        await DropUserCache(userId);
                    }
                    break;
                default:
                    logger.LogWarning("Unknown type in cache message: {Type}", envelope.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработке сообщения из кэша");
        }
    }

    private async Task ReloadItemsCache()
    {
        var itemsCache = serviceProvider.GetRequiredService<ItemsCache>();
        await itemsCache.ClearItemsCache();
        var items = await casesCommService.GetAllItems(new DefaultRequest
        {
            Count = int.MaxValue
        });
        await itemsCache.SetAllItemsToCache(items.Result);
    }

    private async Task ReloadCasesCache()
    {
        var casesCache = serviceProvider.GetRequiredService<CasesCache>();
        await casesCache.ClearAllCasesCacheTask();
        var cases = await casesCommService.GetAllCases(new DefaultRequest
        {
            Count = int.MaxValue
        });
        await casesCache.SetCasesToCache(cases.Result);
    }

    private async Task ReloadUsersCache()
    {
        var usersCache = serviceProvider.GetRequiredService<UsersCache>();
        await usersCache.DropAllUsersCache();
        var usersCommService = serviceProvider.GetRequiredService<UsersCommService>();
        var usersResult = await usersCommService.GetAllUsers();

        foreach (var userDto in usersResult.Result)
        {
            await usersCache.SetUserToCache(userDto);
        }

        await usersCache.SetUsersToCache(usersResult.Result);

    }

    private async Task WarmupCache()
    {
        await ReloadCasesCache();
        await ReloadItemsCache();
        await ReloadUsersCache();
    }

    private async Task DropUserCache(string userId)
    {
        var usersCache = serviceProvider.GetRequiredService<UsersCache>();
        await usersCache.DropUserCache(userId);
        await usersCache.DropAllUsersCache();

        var usersCommService = serviceProvider.GetRequiredService<UsersCommService>();

        var user = await usersCommService.GetUserData(userId);

        await usersCache.SetUserToCache(user.Result);

    }
}
