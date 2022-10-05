using Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class ResponseCacheService : IResponseCacheService
{
    private readonly IDatabase database;

    public ResponseCacheService(IConnectionMultiplexer redis)
    {
        database = redis.GetDatabase();
    }

    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        if (response == null)
        {
            return;
        }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        string serializedResponse = JsonSerializer.Serialize(response, options);
        await database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
    }

    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        RedisValue cacheResponse = await database.StringGetAsync(cacheKey);

        if (cacheResponse.IsNullOrEmpty)
        {
            return null;
        }

        return cacheResponse;
    }
}
