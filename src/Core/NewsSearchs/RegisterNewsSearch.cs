using Microsoft.Extensions.Logging;

namespace Core.NewsSearchs;

public sealed class RegisterNewsSearch(ILogger<RegisterNewsSearch> logger, INewsSearchRepository newsSearchRepository)
{
    public async Task Handle(NewsSearch newsSearch)
    {
        logger.LogInformation("Registrando pesquisa de notícias");
        await newsSearchRepository.Create(newsSearch);
    }
}
