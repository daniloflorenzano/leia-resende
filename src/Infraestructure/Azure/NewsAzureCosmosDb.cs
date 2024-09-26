using System.Linq.Expressions;
using Core.News;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infraestructure.Azure;

public sealed class NewsAzureCosmosDb(AppDbContext dbContext, ILogger<NewsAzureCosmosDb> logger) : INewsRepository
{
    public async Task Create(News news)
    {
        try
        {
            var exists = await dbContext.News.FirstOrDefaultAsync(n => n.Title == news.Title);
            if (exists is null){
                await dbContext.News.AddAsync(news);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex) when (ex.Message.Contains("Response status code does not indicate success: NotFound (404)"))
        {
            logger.LogWarning("Notícia já existe no banco de dados {0}, continuando...", news.Title);
            await dbContext.News.AddAsync(news);
            await dbContext.SaveChangesAsync();
            return;
        }
    }

    public async Task CreateRange(News[] news)
    {
        foreach (var n in news)
            await Create(n);
    }

    public async Task<IEnumerable<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        try
        {
            if (where is null)
                return await dbContext.News.ToListAsync();

            return await dbContext.News.Where(where).ToListAsync();
        }
        catch (Exception ex) when (ex.Message.Contains("Response status code does not indicate success: NotFound (404)"))
        {
            logger.LogWarning("Parece que nao ha nada no banco, continuando...");
            return [];
        }        
    }
}
