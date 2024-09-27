using System.Linq.Expressions;
using Core.News;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace Infraestructure.Azure;

public sealed class NewsAzureCosmosDb : INewsRepository
{
    private readonly Container container;

    private readonly ILogger<NewsAzureCosmosDb> logger; 

    public NewsAzureCosmosDb(AzureCosmosDbConfig azureCosmosDbConfig, ILogger<NewsAzureCosmosDb> logger)
    {
        var cosmosClient = new CosmosClient(azureCosmosDbConfig.Endpoint, azureCosmosDbConfig.SecretKey);
        var database = cosmosClient.GetDatabase(azureCosmosDbConfig.DatabaseName);
        container = database.GetContainer("News");

        this.logger = logger;
    }

    public async Task Create(News news) => await AddItemsToContainerAsync(news);

    private async Task AddItemsToContainerAsync(News news) 
    {
        try
        {            
            var payload = new NewsPayload(news.Id.ToString(), news.Title, news.Content, news.Author, news.Subject.ToString(), news.PublishedAt, news.OriginalUrl.ToString(), news.ImageUrl?.ToString());
            var newsResponse = await container.CreateItemAsync(payload);
            logger.LogInformation("Notícia adicionada com sucesso: {0}, id: {1}", payload.title, newsResponse.Resource.id);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            logger.LogWarning("Notícia já existe: {0}", news.Title);
        }
        catch (Exception ex)
        {
            logger.LogError("Erro ao adicionar notícia: {0}", ex.Message);
        }
    }

    public async Task<IEnumerable<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        try
        {
            var query = container.GetItemLinqQueryable<News>(true)
                                 .Where(where ?? (news => true))
                                 .ToFeedIterator();

            var results = new List<News>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogWarning("Parece que nao ha nada no banco, continuando...");
            return [];
        }        
    }
}

public record NewsPayload(string id, string title, string content, string author, string subject, DateTime publishedAt, string originalUrl, string? imageUrl);