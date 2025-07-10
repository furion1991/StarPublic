using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using DtoClassLibrary.DataTransferObjects.Common.Logs;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;
using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.DataTransferObjects.Audit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AuditService.Services;

public class AdminLogListenerService(ILogger<AdminLogListenerService> logger,
    OpenedCasesService openedCasesService, IServiceScopeFactory scopeFactory, IDbContextFactory<ApplicationDbContext> dbContextFactory) : BackgroundService
{
    private const string ExchangeName = "admin.logs.exchange";
    private const string QueueName = "admin.dashboard.logs";

    private IConnection? _connection;
    private IChannel? _channel;

    private readonly ConnectionFactory _factory = new()
    {
        HostName = "rabbitmq",
        UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
        Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest"
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await EstablishConnectionWithRetry(stoppingToken);
        _channel = await _connection!.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "admin.log.*", cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceived;

        await _channel.BasicConsumeAsync(queue: QueueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs ea)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var json = Encoding.UTF8.GetString(ea.Body.ToArray());

        try
        {
            var raw = JsonSerializer.Deserialize<AdminLogRaw>(json);

            switch (raw.Type.ToLower())
            {
                case "casesopendto":
                    var caseLog = JsonConvert.DeserializeObject<AdminLog<CasesOpenDto>>(json);
                    logger.LogInformation("[AdminLog] Кейс открыт на {0}", caseLog?.Payload.Amount);
                    break;

                case "contractdashboarddata":
                    var contractLog = JsonConvert.DeserializeObject<AdminLog<ContractDashboardData>>(json);
                    logger.LogInformation("[AdminLog] Контракт профит {0}", contractLog?.Payload.Profit);
                    break;
                case "dashboardbalancedto":
                    {
                        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
                        var adminLog = JsonConvert.DeserializeObject<AdminLog<DashboardBalanceDto>>(json);
                        await dbContext.BalanceStatisticsRecords.AddAsync(new BalanceStatisticsRecord()
                        {
                            AllUsersActualBalance = adminLog.Payload.AllUsersBalance,
                            AllUsersBonusBalance = adminLog.Payload.AllUsersBonusBalance,
                            RecordTimeStamp = DateTime.UtcNow,
                            TotalProfit = adminLog.Payload.TotalProfit,
                            Withdrawn = adminLog.Payload.AmountWithdrawn

                        });
                        await dbContext.SaveChangesAsync();
                        break;
                    }
                default:
                    logger.LogWarning("[AdminLog] Неизвестный тип: {0}", raw.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обработки сообщения: {Json}", json);
        }

        await Task.CompletedTask;
    }



    private async Task EstablishConnectionWithRetry(CancellationToken cancellationToken, int maxRetries = 10, int delayMs = 2000)
    {
        int attempts = 0;
        while (attempts < maxRetries && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                logger.LogInformation("[AdminLog] Соединение с RabbitMQ установлено.");
                return;
            }
            catch (BrokerUnreachableException)
            {
                logger.LogWarning("[AdminLog] Попытка {0} подключения не удалась. Повтор через {1} мс", attempts, delayMs);
                attempts++;
                await Task.Delay(delayMs, cancellationToken);
            }
        }

        if (_connection == null)
        {
            logger.LogError("[AdminLog] Не удалось подключиться к RabbitMQ.");
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }



    public async Task UpdateDashboard(IServiceScope scope)
    {
        var dashboardData = await GetDashboardData();

        var hubService = scope.ServiceProvider.GetRequiredService<DashboardService>();

        await hubService.UpdateDashboardAsync(dashboardData);
    }
    private async Task<DashboardDto> GetDashboardData()
    {
        var dbContext = await dbContextFactory.CreateDbContextAsync();
        var userBalanceData = dbContext.BalanceStatisticsRecords
            .OrderByDescending(e => e.RecordTimeStamp)
            .FirstOrDefault();

        var finLogs = dbContext.FinancialLogs
            .Where(e => e.FinancialLogType == FinansialLogDto.FTYPE.Deposit).ToList();

        var dashboardDto = new DashboardDto
        {
            CasesOpened = await openedCasesService.GetOpenedCasesStatsForAdmin(),
            UsersBalanceData = new DashboardBalanceDto()
            {
                AllUsersBonusBalance = userBalanceData.AllUsersBonusBalance,
                TotalProfit = userBalanceData.TotalProfit,
                AllUsersBalance = userBalanceData.AllUsersActualBalance,
                AmountWithdrawn = userBalanceData.Withdrawn
            },
            ContractData = null,
            CrashData = null,
            MajorDepositDto = null,
            UpgradeData = null,
            UsersDashBoardData = null,
        };

        return dashboardDto;
    }
    private class AdminLogRaw
    {
        public string Type { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string ActionPerformedBy { get; set; } = default!;
        public JsonElement Payload { get; set; }
    }
}
