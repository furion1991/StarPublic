using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.DataTransferObjects.Audit;
using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Audit;
using DtoClassLibrary.DataTransferObjects.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Log = DataTransferLib.DataTransferObjects.Common.Log;

namespace AuditService.Services
{
    public class ListenerService(IServiceScopeFactory scopeFactory, IDbContextFactory<ApplicationDbContext> dbContextFactory,
        OpenedCasesService openedCasesService, ILogger<ListenerService> logger) : BackgroundService
    {
        private readonly List<LTYPE> LOG_TYPES = [LTYPE.Case, LTYPE.Financial, LTYPE.Item, LTYPE.User];

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            foreach (LTYPE logType in LOG_TYPES)
            {
                ConnectionFactory factory = new()
                {
                    HostName = "rabbitmq",
                    UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "",
                    Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "",
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(10)
                };

                IConnection connection = null;
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        connection = await factory.CreateConnectionAsync(stoppingToken);
                        break;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Retrying connection...");
                        await Task.Delay(5000, stoppingToken);
                    }
                }

                if (connection == null)
                {
                    throw new Exception("Unable to connect to RabbitMQ after multiple attempts.");
                }

                IChannel channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
                string queueName = $"log_{logType}";

                await channel.ExchangeDeclareAsync(
                    exchange: RabbitMqConfig.LogsExchangeName,
                    type: RabbitMqConfig.ExchangeType,
                    durable: RabbitMqConfig.Durable,
                    autoDelete: RabbitMqConfig.AutoDelete,
                    cancellationToken: stoppingToken);

                var queue = await channel.QueueDeclareAsync(queue: queueName,
                    durable: RabbitMqConfig.Durable,
                    exclusive: RabbitMqConfig.Exclusive,
                    autoDelete: RabbitMqConfig.AutoDelete,
                    cancellationToken: stoppingToken);

                await channel.QueueBindAsync(queue: queue.QueueName, exchange: RabbitMqConfig.LogsExchangeName, routingKey: queueName, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += HandleMessages;

                await channel.BasicConsumeAsync(queueName, false, consumer, cancellationToken: stoppingToken);
            }

            await Task.CompletedTask;
        }



        private async Task HandleMessages(object ch, BasicDeliverEventArgs ea)
        {
            logger.LogInformation($"Received message on queue: {ea.RoutingKey}");
            var channel = ((AsyncEventingBasicConsumer)ch).Channel;
            try
            {
                Log? log = JsonSerializer.Deserialize<Log>(Encoding.UTF8.GetString(ea.Body.ToArray()));

                if (log == null)
                    return;

                await using var scope = scopeFactory.CreateAsyncScope();
                LogService logService = scope.ServiceProvider.GetRequiredService<LogService>();
                BaseLog? baseLog = await logService.Add(log);

                if (log.LogType == LTYPE.Case)
                {
                    await HandleCaseLog(scope, log);
                }

                if (log.LogType == LTYPE.User && (log.Message.IndexOf("user logged out", StringComparison.Ordinal) != -1
                                                  || log.Message.IndexOf("user logged in", StringComparison.Ordinal) != -1))
                {
                    await HandleUserLoginLog(scope, log);
                }

                if (log.LogType == LTYPE.Financial)
                {

                }

                await channel.BasicAckAsync(ea.DeliveryTag, false);
                logger.LogInformation($"Message on {ea.RoutingKey} handled successfully.");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        private async Task UpdateDashboard(IServiceScope scope)
        {
            var dashboard = scope.ServiceProvider.GetRequiredService<IHubContext<DashboardHub>>();

        }
        private async Task HandleUserLoginLog(IServiceScope scope, Log log)
        {
            IHubContext<Sender> hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<Sender>>();

            var usersCount = JsonConvert.DeserializeObject<int>(log.Content.ToString());
            await hubContext.Clients.All.SendAsync(Sender.LoggedInUsers, usersCount);
        }


        private async Task HandleFinancialLog(IServiceScope scope, Log log)
        {
            if (log.Content is JsonElement jsonElement)
            {
                var finLog = jsonElement.Deserialize<FinansialLogDto>();
                
            }
        }


        private async Task HandleCaseLog(IServiceScope scope, Log log)
        {
            IHubContext<Sender> hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<Sender>>();
            if (log.Message.IndexOf("opened case", StringComparison.Ordinal) != -1 || log.Message.IndexOf("contract executed", StringComparison.Ordinal) != -1)
            {
                if (log.Content is JsonElement jsonElement)
                {
                    var caseDto = jsonElement.Deserialize<DroppedItemDto>();

                    if (caseDto != null)
                    {
                        await SaveOpenedCaseAsync(caseDto);
                    }
                }
                else if (log.Content is string jsonString)
                {
                    var caseDto = JsonConvert.DeserializeObject<DroppedItemDto>(jsonString);
                    if (caseDto != null)
                    {
                        await SaveOpenedCaseAsync(caseDto);
                    }
                }
            }
            else if (log.Message.IndexOf("opened multiple cases", StringComparison.Ordinal) != -1)
            {
                if (log.Content is string jsonContent)
                {
                    var openedCasesList = JsonConvert.DeserializeObject<List<DroppedItemDto>>(jsonContent);

                    foreach (var item in openedCasesList)
                    {
                        await SaveOpenedCaseAsync(item);
                    }
                }
                else if (log.Content is JsonElement jsonElement)
                {
                    var openedCasesList = jsonElement.Deserialize<List<DroppedItemDto>>();
                    if (openedCasesList != null)
                    {
                        foreach (var item in openedCasesList)
                        {
                            await SaveOpenedCaseAsync(item);
                        }
                    }
                }
            }

            var context = await dbContextFactory.CreateDbContextAsync();

            var lastItemsDropped = await openedCasesService.GetLastTenDroppedItems();
            var droppedItemsByCost = await openedCasesService.GetLastTenItemsDroppedByCost();
            var casesOpenedCount = context.OpenedCases.Count();

            await hubContext.Clients.All.SendAsync(Sender.CasesOpenedCount, casesOpenedCount);
            await hubContext.Clients.All.SendAsync(Sender.LastOpenedCases, lastItemsDropped);
            await hubContext.Clients.All.SendAsync(Sender.LastOpenedCasesByCost, droppedItemsByCost);
        }


        private async Task SaveOpenedCaseAsync(DroppedItemDto droppedItemDto)
        {
            var dbContext = await dbContextFactory.CreateDbContextAsync();

            await dbContext.OpenedCases.AddAsync(new OpenedCase()
            {
                CaseId = droppedItemDto.CaseId,
                ItemDroppedId = droppedItemDto.Item.Id,
                OpenedTimeStamp = droppedItemDto.OpenedTimeStamp,
                UserId = droppedItemDto.UserId,
                Cost = droppedItemDto.SellPrice
            });

            await dbContext.SaveChangesAsync();
        }
    }
}