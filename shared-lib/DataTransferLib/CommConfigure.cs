using System.Collections.Immutable;
using DataTransferLib.CacheServices;
using DataTransferLib.CommunicationsServices;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DataTransferLib;

public static class CommConfigure
{
    public const string USERS_CLIENT_NAME = "users";
    public const string AUTH_CLIENT_NAME = "auth";
    public const string CASES_CLIENT_NAME = "cases";
    public const string AUDIT_CLIENT_NAME = "audit";
    public const string FINANCE_CLIENT_NAME = "finance";
    public const string CACHE_CLIENT_NAME = "cache";

    private static ImmutableDictionary<string, string> _clientNames = new Dictionary<string, string>()
    {
        { USERS_CLIENT_NAME, "http://users-service:7005" },
        { AUTH_CLIENT_NAME, "http://authservice:7002" },
        { CASES_CLIENT_NAME, "http://cases-service:7004" },
        { AUDIT_CLIENT_NAME, "http://audit-service:7006" },
        { FINANCE_CLIENT_NAME, "http://financial-service:7009" },
        { CACHE_CLIENT_NAME, "http://cache-service:7008" }
    }.ToImmutableDictionary();

    /// <summary>
    /// Добавляет сервисы для коммуникации между микросервисами
    ///  </summary>
    /// <param name="services"></param>
    public static void AddCommunicationServices(this IServiceCollection services)
    {
        foreach (var clientName in _clientNames)
        {
            services.AddHttpClient(clientName.Key, client => { client.BaseAddress = new Uri(clientName.Value); });
        }

        services.AddScoped<AuditCommService>();
        services.AddScoped<AuthCommService>();
        services.AddScoped<CasesCommService>();
        services.AddScoped<UsersCommService>();
        services.AddScoped<FinancialCommService>();
        services.AddSingleton<RabbitMqService>();
        services.AddScoped<RabbitMqCacheService>();
        services.AddSingleton<AdminLogRabbitMqService>();
        services.AddSingleton(ConnectionMultiplexer.Connect("redis:6379"));
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "redis:6379";
            options.InstanceName = "stardrop";
        });
        services.AddScoped<CacheService>();
        services.AddSingleton<ItemsCache>();
        services.AddSingleton<CasesCache>();
        services.AddSingleton<UsersCache>();
    }
}