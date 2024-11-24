using Core.Application;
using Microsoft.Extensions.Logging;

namespace Core.News;

public sealed class GetNews(ILogger<GetNews> logger, INewsRepository newsRepository)
{
    public async Task<IEnumerable<News>> Handle(SearchFilter? filter = null)
    {     
        logger.LogInformation("Buscando notícias");
        return await newsRepository.Read(filter);
    }
}


