using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DataTransferLib.CacheServices;

public class ItemsCache(IDistributedCache cache, ConnectionMultiplexer redis, ILogger<ItemsCache> logger)
{
    public async Task<ItemDto?> GetItemFromCache(string id)
    {
        logger.LogInformation("➡️ Start: GetItemFromCache({Id})", id);
        var data = await cache.GetStringAsync($"item:{id}");
        if (string.IsNullOrEmpty(data))
        {
            logger.LogWarning("❌ No item found in cache for key item:{Id}", id);
            return null;
        }
        try
        {
            var result = JsonConvert.DeserializeObject<ItemDto>(data);
            logger.LogInformation("✅ Item {Id} deserialized from cache", id);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to deserialize ItemDto from cache for id: {Id}", id);
            return null;
        }
    }

    public async Task SetItemToCache(ItemDto? item)
    {
        logger.LogInformation("➡️ Start: SetItemToCache");
        if (item == null || string.IsNullOrEmpty(item.Id))
        {
            logger.LogWarning("Attempted to cache an item with null or empty Id.");
            return;
        }
        var data = JsonConvert.SerializeObject(item);
        await cache.SetStringAsync($"item:{item.Id}", data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3)
        });
        logger.LogInformation("✅ Cached item with Id: {Id}", item.Id);
    }

    public async Task<List<ItemDto>> GetItemsFromCache(List<string> ids)
    {
        logger.LogInformation("➡️ Start: GetItemsFromCache for {Count} items", ids.Count);
        var items = new List<ItemDto>();
        foreach (var id in ids)
        {
            var item = await GetItemFromCache(id);
            if (item != null)
            {
                items.Add(item);
            }
        }
        logger.LogInformation("✅ End: GetItemsFromCache, found {Count} items", items.Count);
        return items;
    }

    public async Task SetItemsToCache(List<ItemDto> items)
    {
        logger.LogInformation("➡️ Start: SetItemsToCache for {Count} items", items.Count);
        foreach (var item in items)
        {
            await SetItemToCache(item);
        }
        logger.LogInformation("✅ End: SetItemsToCache");
    }

    public async Task<List<ItemDto>?> GetAllItemsFromCache(string? query = "")
    {
        logger.LogInformation("➡️ Start: GetAllItemsFromCache");
        var data = await cache.GetStringAsync($"items:all{query}");
        if (string.IsNullOrEmpty(data))
        {
            logger.LogWarning("❌ items:all not found in cache");
            return null;
        }
        try
        {
            var items = JsonConvert.DeserializeObject<List<ItemDto>>(data);
            logger.LogInformation("✅ Found {Count} items in items:all", items?.Count);
            return items;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to deserialize all items from cache.");
            return null;
        }
    }

    public async Task SetAllItemsToCache(List<ItemDto> items, string? query = "")
    {
        logger.LogInformation("➡️ Start: SetAllItemsToCache with {Count} items", items?.Count);
        if (items == null || items.Count == 0)
        {
            logger.LogWarning("Attempted to cache an empty list of items.");
            return;
        }
        var data = JsonConvert.SerializeObject(items);
        await cache.SetStringAsync($"items:all{query}", data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3)
        });
        logger.LogInformation("✅ Cached items:all");
    }

    public async Task ClearItemsCache()
    {
        logger.LogInformation("➡️ Start: ClearItemsCache");
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(pattern: "item:*");
        foreach (var key in keys)
        {
            logger.LogInformation("Dropping item cache key: {Key}", key);
            await redis.GetDatabase().KeyDeleteAsync(key);
        }
        var keysList = server.Keys(pattern: "items:*");
        foreach (var key in keysList)
        {
            logger.LogInformation("Dropping items cache key: {Key}", key);
            await redis.GetDatabase().KeyDeleteAsync(key);
        }
        logger.LogInformation("✅ End: ClearItemsCache");
    }
}
