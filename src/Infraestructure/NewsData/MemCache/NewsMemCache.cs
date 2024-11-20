using System.Linq.Expressions;
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

    public async Task Write(News news)
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

    public Task<List<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        var cacheEntries = _cache.GetKeys<News>().ToList();
        
        if (where == null)
        {
            return Task.FromResult(cacheEntries);
        }
        
        var filteredEntries = cacheEntries.Where(where.Compile()).ToList();

        return Task.FromResult(filteredEntries);
    }
}