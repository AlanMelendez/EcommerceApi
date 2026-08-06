using System.Text.Json;
using Ecommerce.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    //It is a interface provided by Redis package to use the different methods.
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IDistributedCache distributedCache,
        ILogger<RedisCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            var cachedValue = await _distributedCache.GetStringAsync(
                key,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(cachedValue))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(cachedValue, JsonOptions);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Redis cache read failed for key {CacheKey}.",
                key);

            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken)
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, JsonOptions);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _distributedCache.SetStringAsync(
                key,
                serializedValue,
                options,
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Redis cache write failed for key {CacheKey}.",
                key);
        }
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Redis cache remove failed for key {CacheKey}.",
                key);
        }
    }

    public async Task<int> GetVersionAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var version = await GetAsync<int>(key, cancellationToken);

        // It checks if the retrieved version is a positive integer. If it is, it returns that version. If not, it sets the version to 1 in the cache and returns 1.
        if (version is int v && v > 0)
        {
            return v;
        }

        await SetAsync(
            key,
            1,
            TimeSpan.FromDays(30),
            cancellationToken);

        return 1;
    }

    public async Task<int> IncrementVersionAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var currentVersion = await GetVersionAsync(key, cancellationToken);

        var newVersion = currentVersion + 1;

        await SetAsync(
            key,
            newVersion,
            TimeSpan.FromDays(30),
            cancellationToken);

        return newVersion;
    }
}