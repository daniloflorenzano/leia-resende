using Core.News;
using Core.NewsSearchs;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.EfCore;

public sealed class AppDbContext : DbContext
{
    public DbSet<News> News { get; set; }
    public DbSet<NewsSearch> NewsSearchs { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
        
        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Author).IsRequired();
            entity.Property(e => e.AuthorIconUrl).IsRequired().HasColumnName("author_icon_url");
            entity.Property(e => e.Subject).IsRequired();
            entity.Property(e => e.PublishedAt).IsRequired().HasColumnName("published_at");
            entity.Property(e => e.OriginalUrl).IsRequired().HasColumnName("original_url");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
        });
        
        modelBuilder.Entity<NewsSearch>(entity =>
        {
            entity.ToTable("news_search");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");
        });
    }
}