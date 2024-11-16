using Core.NewsSearchs;
using Infraestructure.Azure;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Infraestructure.NewsData;

public class NewsSearchAzureCosmosDb : INewsSearchRepository
{
    private readonly Container _container;
    private readonly ILogger<NewsSearchAzureCosmosDb> _logger;

    public NewsSearchAzureCosmosDb(AzureCosmosDbConfig azureCosmosDbConfig, ILogger<NewsSearchAzureCosmosDb> logger)
    {
        var cosmosClient = new CosmosClient(azureCosmosDbConfig.Endpoint, azureCosmosDbConfig.SecretKey);
        var database = cosmosClient.GetDatabase(azureCosmosDbConfig.DatabaseName);
        _container = database.GetContainer("NewsSearch");

        this._logger = logger;
    }

    public async Task Create(NewsSearch newsSearch)
    {
        try
        {
            var payload = new NewsSearchPayload(newsSearch.Id.ToString(), newsSearch.CreatedAt);
            var newsSearchResponse = await _container.CreateItemAsync(payload);
            _logger.LogInformation("Busca de notícia adicionada com sucesso, id: {1}", newsSearchResponse.Resource.id);
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao adicionar notícia: {message}", ex.Message);
        }        
    }
}

public record NewsSearchPayload(string id, DateTime createdAt);
