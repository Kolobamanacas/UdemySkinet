using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Data;

public class BasketRepository : IBasketRepository
{
    private readonly IDatabase database;

    public BasketRepository(IConnectionMultiplexer redis)
    {
        database = redis.GetDatabase();

    }

    public async Task<CustomerBasket?> GetBasketAsync(string basketId)
    {
        RedisValue value = await database.StringGetAsync(basketId);
        return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(value);
    }

    public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
    {
        string serializedBasket = JsonSerializer.Serialize(basket);
        bool isCreated = await database.StringSetAsync(basket.Id, serializedBasket, TimeSpan.FromDays(30));

        if (!isCreated)
        {
            return null;
        }

        return await GetBasketAsync(basket.Id);
    }

    public async Task<bool> DeleteBasketAsync(string basketId)
    {
        return await database.KeyDeleteAsync(basketId);
    }
}
