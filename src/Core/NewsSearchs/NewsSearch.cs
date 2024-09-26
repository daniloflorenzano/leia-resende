using Core.News;

namespace Core.NewsSearchs;

public sealed class NewsSearch()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime SearchAt { get; set; } = DateTime.Now;

    public async Task StartSearch()
    {
        var scrapNews = ScrapNews.GetInstance();
        await scrapNews.Handle();
    }
}
