using Core.Application;
using Microsoft.Extensions.Logging;

namespace Core.News;

public sealed class GetNews(ILogger<GetNews> logger, INewsRepository newsRepository)
{
    private readonly GlobalInfo _globalInfo = GlobalInfo.GetInstance();
    
    public async Task<IEnumerable<News>> Handle(SearchFilter? filter = null)
    {     
        logger.LogInformation("Buscando notícias");
        var result = await newsRepository.Read(filter);
        
        await UpdateTotalNewsIfNecessary();
        await UpdateAvailableSubjectsIfNecessary();

        return result;
    }

    private async Task UpdateTotalNewsIfNecessary()
    {
        if (_globalInfo.TotalNews is 0) 
            _globalInfo.TotalNews = await newsRepository.Count();
    }
    
    private async Task UpdateAvailableSubjectsIfNecessary()
    {
        if (_globalInfo.AvailableSubjects.Count is 0) 
            _globalInfo.AvailableSubjects = await newsRepository.GetAvailableSubjects();
    }
}


