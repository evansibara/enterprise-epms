using EPMS.Application.Interfaces.Services;
using StackExchange.Redis;

namespace EPMS.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    private IDatabase Database => _connectionMultiplexer.GetDatabase();

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await Database.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetStringAsync(
        string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) =>
        await Database.StringSetAsync(key, value, expiry);

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        await Database.KeyDeleteAsync(key);

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default) =>
        await Database.KeyExistsAsync(key);
}
