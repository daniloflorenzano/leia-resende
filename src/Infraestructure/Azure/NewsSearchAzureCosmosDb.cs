using Core.NewsSearchs;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Infraestructure.Azure;

public class NewsSearchAzureCosmosDb : INewsSearchRepository
{
    private readonly Container container;
    private readonly ILogger<NewsSearchAzureCosmosDb> logger;

    public NewsSearchAzureCosmosDb(AzureCosmosDbConfig azureCosmosDbConfig, ILogger<NewsSearchAzureCosmosDb> logger)
    {
        var cosmosClient = new CosmosClient(azureCosmosDbConfig.Endpoint, azureCosmosDbConfig.SecretKey);
        var database = cosmosClient.GetDatabase(azureCosmosDbConfig.DatabaseName);
        container = database.GetContainer("NewsSearch");

        this.logger = logger;
    }

    public async Task Create(NewsSearch newsSearch)
    {
        try
        {
            var payload = new NewsSearchPayload(newsSearch.Id.ToString(), newsSearch.CreatedAt);
            var newsSearchResponse = await container.CreateItemAsync(payload);
            logger.LogInformation("Busca de notícia adicionada com sucesso, id: {1}", newsSearchResponse.Resource.id);
        }
        catch (Exception ex)
        {
            logger.LogError("Erro ao adicionar notícia: {0}", ex.Message);
        }        
    }
}

public record NewsSearchPayload(string id, DateTime createdAt);
