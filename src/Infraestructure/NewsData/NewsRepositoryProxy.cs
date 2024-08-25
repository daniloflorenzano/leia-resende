using System.Linq.Expressions;
using Core.News;
using Infraestructure.Azure;
using Microsoft.Extensions.Logging;

namespace Infraestructure.NewsData;

public sealed class NewsRepositoryProxy(ILogger<NewsRepositoryProxy> logger, NewsMemCache memCacheRepository, NewsAzureCosmosDb dbRepository) : INewsRepository
{
    public async Task Create(News news)
    {
        logger.LogInformation("Salvando notícia no cache {0}", news.Title);
        await WriteNewsToCache(news);

        logger.LogInformation("Salvando notícia no banco de dados {0}", news.Title);
        await WriteNewsToDb(news);
    }

    public async Task<IEnumerable<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        logger.LogInformation("Buscando notícias no cache");
        var newsFromCache = await GetNewsFromCache(where);
        if (newsFromCache != null)
            return newsFromCache;

        logger.LogInformation("Buscando notícias no banco de dados");
        return await GetNewsFromDb(where);
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
