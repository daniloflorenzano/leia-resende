using System.Linq.Expressions;
using Core.News;
using EFCore.BulkExtensions;
using Infraestructure.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infraestructure.NewsData;

public class NewsAzureSqlDatabase(AppDbContext dbContext, ILogger<NewsAzureSqlDatabase> logger) : INewsRepository
{
    public async Task Write(List<News> news)
    {
        if (news.Count == 0) return;

        try
        {
            var bulkConfig = new BulkConfig
            {
                BatchSize = 5000,
                PreserveInsertOrder = false,
                SqlBulkCopyOptions = SqlBulkCopyOptions.Default,
                UpdateByProperties = [ nameof(News.OriginalUrl) ],
                PropertiesToIncludeOnUpdate = [""]
            };

            await dbContext.BulkInsertOrUpdateAsync(news, bulkConfig);

            logger.LogInformation("Bulk insert concluído para {Count} notícias", news.Count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao adicionar notícias: {Message}", e.Message);
            throw;
        }
    }

    public async Task<List<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        try
        {
            return await dbContext.News
                .Where(where ?? (news => true))
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao ler notícias: {message}", e.Message);
            throw;
        }
    }
}