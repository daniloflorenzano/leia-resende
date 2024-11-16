using System.Linq.Expressions;
using Core.News;
using Infraestructure.Azure;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace Infraestructure.NewsData;
 
public sealed class NewsAzureCosmosDb : INewsRepository
{
    private readonly Container _container;
    private readonly ILogger<NewsAzureCosmosDb> _logger; 

    public NewsAzureCosmosDb(AzureCosmosDbConfig azureCosmosDbConfig, ILogger<NewsAzureCosmosDb> logger)
    {
        var cosmosClient = new CosmosClient(azureCosmosDbConfig.Endpoint, azureCosmosDbConfig.SecretKey);
        var database = cosmosClient.GetDatabase(azureCosmosDbConfig.DatabaseName);
        _container = database.GetContainer("News");
        _logger = logger;
    }

    public async Task Create(News news) => await AddItemsToContainerAsync(news);

    private async Task AddItemsToContainerAsync(News news) 
    {
        try
        {            
            var payload = new NewsPayload(news.Id, news.Title, news.Content, news.Author, news.AuthorIconUrl.ToString(), news.Subject.ToString(), news.PublishedAt, news.OriginalUrl.ToString(), news.ImageUrl?.ToString());
            await _container.UpsertItemAsync(payload);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            _logger.LogWarning("Notícia já existe: {title}", news.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao adicionar notícia: {message}", ex.Message);
        }
    }

    public async Task<IEnumerable<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        try
        {
            var query = _container.GetItemLinqQueryable<News>(true)
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
            _logger.LogWarning("Parece que nao ha nada no banco, continuando...");
            return [];
        }        
    }
}

public record NewsPayload(string id, string title, string content, string author, string authorIconUrl, string subject, DateTime publishedAt, string originalUrl, string? imageUrl);