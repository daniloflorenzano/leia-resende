using Core.Application;
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

    public async Task<List<News>> Read(SearchFilter? filter = null)
    {
        logger.LogInformation("Buscando notícias no cache");
        var newsFromCache = await GetNewsFromCache(filter);
        var newsEnumerable = newsFromCache.ToList();
        if (newsEnumerable.Count != 0)
            return newsEnumerable.ToList();

        logger.LogInformation("Buscando notícias no banco de dados");
        var news = await GetNewsFromDb(filter);
        
        await memCacheRepository.Write(news, filter);
        
        return news;
    }

    private async Task<IEnumerable<News>> GetNewsFromCache(SearchFilter? filter = null)
    {
        return await memCacheRepository.Read(filter);
    }

    private async Task<List<News>> GetNewsFromDb(SearchFilter? filter = null)
    {
        return await dbRepository.Read(filter);
    }
}
