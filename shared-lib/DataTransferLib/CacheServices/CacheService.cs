using DataTransferLib.DataTransferObjects.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DataTransferLib.CacheServices;

public enum EntityType
{
    Case,
    User,
    Item,
    Financial,
}

public class CacheService(IDistributedCache cache, ConnectionMultiplexer redis, ILogger<CacheService> logger)
{
    public async Task CacheEntity<T>(T entity) where T : class?
    {
        var key = $"{typeof(T).Name}_{GetEntityId(entity)}";
        await cache.SetStringAsync(key, JsonConvert.SerializeObject(entity));
    }

    public async Task DropCacheByTypeAsync(DefaultRequest.RequestType type)
    {
        var pattern = GetPattern(type);

        await DropKeysByPatternAsync(pattern);
    }

    private string GetPattern(DefaultRequest.RequestType type)
    {
        var pattern = type switch
        {
            DefaultRequest.RequestType.User => "stardrop:userdto*",
            DefaultRequest.RequestType.Cases => "stardrop:casedto*",
            DefaultRequest.RequestType.Items => "stardrop:itemdto*",
            DefaultRequest.RequestType.Financials => "stardrop:transactiondto*",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        return pattern;
    }


    private async Task DropKeysByPatternAsync(string pattern)
    {
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);

        foreach (var key in keys)
        {
            logger.LogInformation($"Dropping keys for {key}");
            await redis.GetDatabase().KeyDeleteAsync(key); // Удаление напрямую через ConnectionMultiplexer
        }
    }



    public async Task<T?> GetCachedEntity<T>(string id) where T : class
    {
        var key = $"{typeof(T).Name}_{id}";
        var cachedString = await cache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cachedString))
        {
            return JsonConvert.DeserializeObject<T>(cachedString);
        }

        return null;
    }

    public async Task DropCachedEntity<T>(string id) where T : class
    {
        var key = $"{typeof(T).Name}_{id}";
        await cache.RemoveAsync(key);
    }

    private string GetEntityId<T>(T entity)
    {
        // Используем reflection, чтобы извлечь значение свойства "Id"
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
        {
            throw new ArgumentException("Сущность не содержит свойства 'Id'");
        }

        var idValue = idProperty.GetValue(entity)?.ToString();
        if (string.IsNullOrEmpty(idValue))
        {
            throw new ArgumentException("Значение 'Id' не может быть пустым");
        }

        return idValue;
    }
    private string FormatKey(string entityType, string listKey)
    {
        return $":{entityType.ToLower()}:{listKey.Replace("_", ":")}";
    }


    public async Task CacheEntityList<T>(List<T> entities, string listKey) where T : class
    {
        var key = FormatKey(typeof(T).Name.ToLower(), listKey);
        await cache.SetStringAsync(key, JsonConvert.SerializeObject(entities));
    }


    public async Task<List<T>?> GetCachedEntityList<T>(string listKey) where T : class
    {
        var key = FormatKey(typeof(T).Name.ToLower(), listKey);
        var cachedString = await cache.GetStringAsync(key);
        return string.IsNullOrEmpty(cachedString) ? null : JsonConvert.DeserializeObject<List<T>>(cachedString);
    }


    public async Task DropCachedEntityList<T>(string listKey) where T : class
    {
        var key = FormatKey(typeof(T).Name.ToLower(), listKey);
        await cache.RemoveAsync(key);
    }

}