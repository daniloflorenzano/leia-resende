using Core.NewsSearchs;
using Infraestructure.EfCore;
using Microsoft.Extensions.Logging;

namespace Infraestructure.NewsData;

public class NewsSearchAzureSqlDatabase(AppDbContext dbContext, ILogger<NewsSearchAzureSqlDatabase> logger) : INewsSearchRepository
{
    public async Task Create(NewsSearch newsSearch)
    {
        try
        {
            await dbContext.NewsSearchs.AddAsync(newsSearch);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao adicionar busca de notícia: {message}", e.Message);
            throw;
        }
    }
}