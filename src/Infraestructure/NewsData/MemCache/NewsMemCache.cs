using Core.Application;
using Core.News;
using Microsoft.Extensions.Caching.Memory;

namespace Infraestructure.NewsData.MemCache;

public sealed class NewsMemCache
{
    private const int CacheExpirationHours = 3;

    // REFERENCIA: https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-8.0#use-setsize-size-and-sizelimit-to-limit-cache-size
    private readonly MemoryCache _cache = new(
        new MemoryCacheOptions
        {
            SizeLimit = 1024,
            ExpirationScanFrequency = TimeSpan.FromHours(CacheExpirationHours)
        }
    );

    public async Task Write(List<News> news, SearchFilter? filter = null)
    {
        var cacheKey = filter?.GetIdentifier() ?? 0;
        await _cache.GetOrCreateAsync(
            cacheKey, cacheEntry =>
            {
                cacheEntry.Size = CalculateApproximateSize(news);
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(CacheExpirationHours);

                _cache.TrackKey(cacheKey);

                return Task.FromResult(news);
            }
        );
    }

    private static int CalculateApproximateSize(List<News> news)
    {
        var size = 1; // Tamanho mínimo

        foreach (var item in news)
        {
            if (!string.IsNullOrEmpty(item.Title))
                size += item.Title.Length / 100; // Divide por 100 para não inflar demais

            if (!string.IsNullOrEmpty(item.Content))
                size += item.Content.Length / 1000; // Divide por 1000 para grandes textos
        }

        return Math.Max(1, size); // Garantir que o tamanho seja pelo menos 1
    }

    public Task<List<News>> Read(SearchFilter? filter = null)
    {
        if (filter is null)
        {
            var cacheEntries = _cache.Get<List<News>>(0);
            return Task.FromResult(cacheEntries ?? []);
        }

        var cacheKey = filter.GetIdentifier();
        var cacheEntry = _cache.Get<List<News>>(cacheKey);

        return Task.FromResult(cacheEntry ?? []);
    }
}