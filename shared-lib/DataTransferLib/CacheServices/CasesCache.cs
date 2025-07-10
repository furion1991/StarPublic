using DtoClassLibrary.DataTransferObjects.CasesItems;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DataTransferLib.CacheServices;

public class CasesCache(IDistributedCache cache, ConnectionMultiplexer redis, ILogger<CasesCache> logger)
{
    public async Task<CaseDto?> GetCaseFromCache(string id)
    {
        logger.LogInformation("➡️ Start: GetCaseFromCache({Id})", id);
        var data = await cache.GetStringAsync($"case:{id}");
        if (string.IsNullOrEmpty(data))
        {
            logger.LogWarning("❌ No case found in cache for key case:{Id}", id);
            return null;
        }
        try
        {
            var result = JsonConvert.DeserializeObject<CaseDto>(data);
            logger.LogInformation("✅ Case {Id} deserialized from cache", id);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to deserialize CaseDto from cache for id: {Id}", id);
            return null;
        }
    }

    public async Task SetCaseToCache(CaseDto? caseDto)
    {
        logger.LogInformation("➡️ Start: SetCaseToCache");
        if (caseDto == null || string.IsNullOrEmpty(caseDto.Id))
        {
            logger.LogWarning("Attempted to cache a case with null or empty Id.");
            return;
        }
        var data = JsonConvert.SerializeObject(caseDto);
        await cache.SetStringAsync($"case:{caseDto.Id}", data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3)
        });
        logger.LogInformation("✅ Cached case with Id: {Id}", caseDto.Id);
    }

    public async Task<List<CaseDto>> GetCasesFromCache(List<string> ids)
    {
        logger.LogInformation("➡️ Start: GetCasesFromCache for {Count} cases", ids.Count);
        var cases = new List<CaseDto>();
        foreach (var id in ids)
        {
            var caseDto = await GetCaseFromCache(id);
            if (caseDto != null)
            {
                cases.Add(caseDto);
            }
        }
        logger.LogInformation("✅ End: GetCasesFromCache, found {Count} cases", cases.Count);
        return cases;
    }

    public async Task SetCasesToCache(List<CaseDto> cases, string query = null)
    {
        logger.LogInformation("➡️ Start: SetCasesToCache for {Count} cases", cases.Count);
        foreach (var caseDto in cases)
        {
            await SetCaseToCache(caseDto);
        }
        var allCasesData = JsonConvert.SerializeObject(cases);
        await cache.SetStringAsync($"cases:all{query}", allCasesData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3)
        });
        logger.LogInformation("✅ End: SetCasesToCache");
    }

    public async Task<List<CaseDto>?> GetAllCasesFromCache(string? query = null)
    {
        logger.LogInformation("➡️ Start: GetAllCasesFromCache");
        var data = await cache.GetStringAsync($"cases:all{query}");
        if (string.IsNullOrEmpty(data))
        {
            logger.LogWarning("❌ cases:all not found in cache");
            return null;
        }
        try
        {
            var cases = JsonConvert.DeserializeObject<List<CaseDto>>(data);
            logger.LogInformation("✅ Found {Count} cases in cases:all", cases?.Count);
            return cases;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to deserialize list of CaseDto from cache");
            return null;
        }
    }

    public async Task ClearAllCasesCacheTask()
    {
        logger.LogInformation("➡️ Start: ClearAllCasesCacheTask");
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(pattern: "case:*");
        foreach (var key in keys)
        {
            logger.LogInformation("Dropping case cache key: {Key}", key);
            await redis.GetDatabase().KeyDeleteAsync(key);
        }
        var keysList = server.Keys(pattern: "cases:*");
        foreach (var key in keysList)
        {
            logger.LogInformation("Dropping cases cache key: {Key}", key);
            await redis.GetDatabase().KeyDeleteAsync(key);
        }
        logger.LogInformation("✅ End: ClearAllCasesCacheTask");
    }
}
