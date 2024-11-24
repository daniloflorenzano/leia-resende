using Core.Application;
using Microsoft.Extensions.Logging;

namespace Core.News;

public sealed class GetNews(ILogger<GetNews> logger, INewsRepository newsRepository)
{
    public async Task<IEnumerable<News>> Handle(SearchFilter? filter = null)
    {     
        logger.LogInformation("Buscando notícias");
        var result = await newsRepository.Read(filter);
        
        await UpdateTotalNewsIfNecessary();

        return result;
    }

    private async Task UpdateTotalNewsIfNecessary()
    {
        var globalInfo = GlobalInfo.GetInstance();
        if (globalInfo.TotalNews is 0) 
            globalInfo.TotalNews = await newsRepository.Count();
    }
}


