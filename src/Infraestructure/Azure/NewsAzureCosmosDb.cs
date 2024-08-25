using System.Linq.Expressions;
using Core.News;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Azure;

public sealed class NewsAzureCosmosDb(AppDbContext dbContext) : INewsRepository
{
    public async Task Create(News news) => await dbContext.News.AddAsync(news);

    public async Task<IEnumerable<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        if (where is null)
            return await dbContext.News.ToListAsync();

        return await dbContext.News.Where(where).ToListAsync();
    }
}
