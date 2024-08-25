using Core.NewsSearchs;

namespace Infraestructure.NewsSearchData;

public sealed class NewsSearchRepository(AppDbContext dbContext) : INewsSearchRepository
{
    public async Task Create(NewsSearch newsSearch)
    {
        await dbContext.NewsSearchs.AddAsync(newsSearch);
    }
}
