using System.Linq.Expressions;
using Core.News;
using Infraestructure.NewsData.MemCache;
using Microsoft.Extensions.Logging;

namespace Infraestructure.NewsData;

public sealed class NewsRepositoryProxy(ILogger<NewsRepositoryProxy> logger, NewsMemCache memCacheRepository, NewsAzureSqlDatabase dbRepository) : INewsRepository
{
    public async Task Write(List<News> news)
    {
        await dbRepository.Write(news);
        await memCacheRepository.Write(news);
    }

    public async Task<List<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        logger.LogInformation("Buscando notícias no cache");
        var newsFromCache = await GetNewsFromCache(where);
        var newsEnumerable = newsFromCache.ToList();
        if (newsEnumerable.Count != 0)
            return newsEnumerable.ToList();

        logger.LogInformation("Buscando notícias no banco de dados");
        var news = await GetNewsFromDb(where);
        foreach (var newsItem in news) 
            await memCacheRepository.Write(news);
        
        return news;
    }

    private async Task<IEnumerable<News>> GetNewsFromCache(Expression<Func<News, bool>>? where = null)
    {
        return await memCacheRepository.Read(where);
    }

    private async Task<List<News>> GetNewsFromDb(Expression<Func<News, bool>>? where = null)
    {
        return await dbRepository.Read(where);
    }
}
