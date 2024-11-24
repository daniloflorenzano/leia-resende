using Core.Application;
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

    public async Task<List<News>> Read(SearchFilter? filter = null)
    {
        try
        {
            var query = dbContext.News.AsQueryable();

            if (filter is null) 
                return await query.ToListAsync();
            
            if (filter.Where is not null) 
                query = query.Where(filter.Where);
            
            if (filter.OrderBy is not null)
                query = filter.OrderByDescending 
                    ? query.OrderByDescending(filter.OrderBy) 
                    : query.OrderBy(filter.OrderBy);

            if (filter is { PaginationStart: >= 0, PaginationEnd: > 0 }) 
                query = query.Skip(filter.PaginationStart).Take(filter.PaginationEnd);

            return await query.ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao ler notícias: {message}", e.Message);
            throw;
        }
    }
}