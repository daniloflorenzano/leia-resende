using Core.News;

namespace Core.NewsSearchs;

public sealed class NewsSearch()
{
    public async Task StartSearch()
    {
        var scrapNews = ScrapNews.GetInstance();
        await scrapNews.Handle();
    }
}
