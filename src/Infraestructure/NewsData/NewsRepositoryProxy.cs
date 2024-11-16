using System.Linq.Expressions;
using Core.News;
using Infraestructure.Azure;
using Infraestructure.NewsData.MemCache;
using Microsoft.Extensions.Logging;

namespace Infraestructure.NewsData;

public sealed class NewsRepositoryProxy(ILogger<NewsRepositoryProxy> logger, NewsMemCache memCacheRepository, NewsAzureCosmosDb dbRepository) : INewsRepository
{
    public async Task Create(News news)
    {
        await WriteNewsToCache(news);
        await WriteNewsToDb(news);
    }

    public async Task<IEnumerable<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        logger.LogInformation("Buscando notícias no cache");
        var newsFromCache = await GetNewsFromCache(where);
        var newsEnumerable = newsFromCache as News[] ?? newsFromCache.ToArray();
        if (newsEnumerable.Length != 0)
            return newsEnumerable;

        logger.LogInformation("Buscando notícias no banco de dados");
        var news = await GetNewsFromDb(where);
        var newsItems = news as News[] ?? news.ToArray();
        foreach (var newsItem in newsItems) 
            await WriteNewsToCache(newsItem);
        
        return newsItems;
    }

    private async Task<IEnumerable<News>> GetNewsFromCache(Expression<Func<News, bool>>? where = null)
    {
        return await memCacheRepository.Read(where);
    }

    private async Task<IEnumerable<News>> GetNewsFromDb(Expression<Func<News, bool>>? where = null)
    {
        return await dbRepository.Read(where);
    }

    private async Task WriteNewsToCache(News news)
    {
        await memCacheRepository.Create(news);
    }

    private async Task WriteNewsToDb(News news)
    {
        await dbRepository.Create(news);
    }
}
