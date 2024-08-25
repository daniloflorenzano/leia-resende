using Core.News;
using Core.NewsSearchs;
using Infraestructure.Azure;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure;

public class AppDbContext(AzureCosmosDbConfig cosmosDbConfig) : DbContext
{
    public DbSet<News> News { get; set; } = null!;
    public DbSet<NewsSearch> NewsSearchs { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseCosmos(
                accountEndpoint: cosmosDbConfig.Endpoint,
                accountKey: cosmosDbConfig.SecretKey,
                databaseName: cosmosDbConfig.DatabaseName
            );
}
