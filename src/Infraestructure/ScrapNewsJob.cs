using Core.NewsSearchs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infraestructure;

public sealed class ScrapNewsJob(ILogger<ScrapNewsJob> logger, RegisterNewsSearch registerNewsSearch) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Iniciando pesquisa de notícias");
        var search = new NewsSearch();
        await search.StartSearch();

        logger.LogInformation("Pesquisa de notícias finalizada");

        await registerNewsSearch.Handle(search);

        logger.LogInformation("Pesquisa de notícias registrada");
    }
}
