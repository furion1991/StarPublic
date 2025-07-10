using DtoClassLibrary.DataTransferObjects.Users;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DataTransferLib.CacheServices;

public class UsersCache(IDistributedCache cache, ConnectionMultiplexer redis, ILogger<UsersCache> logger)
{
    public async Task<UserDto?> GetUserFromCache(string id)
    {
        logger.LogInformation("➡️ Start: GetUserFromCache({Id})", id);
        var data = await cache.GetStringAsync($"user:{id}");
        if (string.IsNullOrEmpty(data))
        {
            logger.LogWarning("❌ No user data found in cache for key user:{Id}", id);
            return null;
        }
        try
        {
            var result = JsonConvert.DeserializeObject<UserDto>(data);
            logger.LogInformation("✅ User {Id} deserialized from cache", id);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to deserialize UserDto from cache for id: {Id}", id);
            return null;
        }
    }

    public async Task SetUserToCache(UserDto? user)
    {
        logger.LogInformation("➡️ Start: SetUserToCache");
        if (user == null || string.IsNullOrEmpty(user.Id))
        {
            logger.LogWarning("Attempted to cache a user with null or empty Id.");
            return;
        }
        var data = JsonConvert.SerializeObject(user);
        await cache.SetStringAsync($"user:{user.Id}", data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
        });
        logger.LogInformation("✅ Cached user with Id: {Id}", user.Id);
    }

    public async Task<List<UserDto>> GetUsersFromCache(List<string> ids)
    {
        logger.LogInformation("➡️ Start: GetUsersFromCache for {Count} users", ids.Count);
        var users = new List<UserDto>();
        foreach (var id in ids)
        {
            var user = await GetUserFromCache(id);
            if (user != null)
            {
                users.Add(user);
            }
        }
        logger.LogInformation("✅ End: GetUsersFromCache, found {Count} users", users.Count);
        return users;
    }

    public async Task<List<UserDto>?> GetAllUsersFromCache()
    {
        logger.LogInformation("➡️ Start: GetAllUsersFromCache");
        var data = await cache.GetStringAsync("users:all");
        if (string.IsNullOrEmpty(data))
        {
            logger.LogWarning("❌ users:all not found in cache");
            return null;
        }
        try
        {
            var result = JsonConvert.DeserializeObject<List<UserDto>>(data);
            logger.LogInformation("✅ Found {Count} users in users:all", result?.Count);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to deserialize List<UserDto> from cache for all users.");
            return null;
        }
    }


    public async Task SetUsersToCache(List<UserDto> users)
    {
        logger.LogInformation("➡️ Start: SetUsersToCache for {Count} users", users.Count);

        await Task.WhenAll(users.Select(SetUserToCache));

        var allUsersData = JsonConvert.SerializeObject(users);
        await cache.SetStringAsync("users:all", allUsersData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
        });
        logger.LogInformation("✅ End: SetUsersToCache");
    }

    public async Task DropUserCache(string id)
    {
        logger.LogInformation("➡️ Start: DropUserCache for Id: {Id}", id);
        if (string.IsNullOrEmpty(id))
        {
            logger.LogWarning("Attempted to drop cache for a user with null or empty Id.");
            return;
        }
        var key = $"user:{id}";
        await cache.RemoveAsync(key);
        logger.LogInformation("✅ Dropped cache for user with Id: {Id}", id);
    }

    public async Task DropUsersCache(List<string> ids)
    {
        logger.LogInformation("➡️ Start: DropUsersCache for {Count} users", ids.Count);
        foreach (var id in ids)
        {
            await DropUserCache(id);
        }
        logger.LogInformation("✅ End: DropUsersCache");
    }

    public async Task DropAllUsersCache()
    {
        logger.LogInformation("➡️ Start: DropAllUsersCache");
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(pattern: "user:*");
        foreach (var key in keys)
        {
            logger.LogInformation("Dropping cache for user with key: {Key}", key);
            await redis.GetDatabase().KeyDeleteAsync(key);
        }
        var keysList = server.Keys(pattern: "users:*");
        foreach (var key in keysList)
        {
            logger.LogInformation("Dropping cache for user list key: {Key}", key);
            await redis.GetDatabase().KeyDeleteAsync(key);
        }
        logger.LogInformation("✅ End: DropAllUsersCache");
    }
}