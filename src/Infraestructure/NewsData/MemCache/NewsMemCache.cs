using Core.Application;
using Core.News;
using Microsoft.Extensions.Caching.Memory;

namespace Infraestructure.NewsData.MemCache;

public sealed class NewsMemCache : INewsRepository
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

    private async Task Write(News news)
    {
        await _cache.GetOrCreateAsync(
            news.Id, cacheEntry =>
            {
                cacheEntry.Size = CalculateApproximateSize(news);
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(CacheExpirationHours);
                
                _cache.TrackKey(news.Id);

                return Task.FromResult(news);
            }
        );
    }

    public async Task Write(List<News> news)
    {
        foreach (var newsItem in news) await Write(newsItem);
    }

    private static int CalculateApproximateSize(News news)
    {
        var size = 1; // Tamanho mínimo

        if (!string.IsNullOrEmpty(news.Title))
            size += news.Title.Length / 100; // Divide por 100 para não inflar demais

        if (!string.IsNullOrEmpty(news.Content))
            size += news.Content.Length / 1000; // Divide por 1000 para grandes textos

        return Math.Max(1, size); // Garantir que o tamanho seja pelo menos 1
    }

    public Task<List<News>> Read(SearchFilter? filter = null)
    {
        var cacheEntries = _cache.GetKeys<News>().ToList();
        
        if (filter is null)
            return Task.FromResult(cacheEntries);
        
        if (filter.Where is not null) 
            cacheEntries = cacheEntries.Where(filter.Where.Compile()).ToList();
        
        if (filter.OrderBy is not null)
            cacheEntries = filter.OrderByDescending
                ? cacheEntries.OrderByDescending(filter.OrderBy.Compile()).ToList()
                : cacheEntries.OrderBy(filter.OrderBy.Compile()).ToList();
        
        if (filter is { PaginationStart: > 0, PaginationEnd: > 0 }) 
            cacheEntries = cacheEntries.Skip(filter.PaginationStart).Take(filter.PaginationEnd).ToList();

        return Task.FromResult(cacheEntries);
    }
}