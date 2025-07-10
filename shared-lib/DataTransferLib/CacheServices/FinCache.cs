using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DataTransferLib.CacheServices;

public class FinCache(IDistributedCache cache, ConnectionMultiplexer redis, ILogger<FinCache> logger)
{

}