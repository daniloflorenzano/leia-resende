using HtmlAgilityPack;

namespace Core.News.DataSources;

public class JornalBeiraRio(HttpClient httpclient)
{
    public async Task<News[]> GetNews(){
        var news = new List<News>();
        news.AddRange(await GetPolitcsNews());
        news.AddRange(await GetPolitcsHealth());
    
        return news.ToArray();
    }

    private async Task<News[]> GetPolitcsNews()
    {
        var response = await httpclient.GetAsync("https://jornalbeirario.com.br/portal/?cat=11");
        var html = await response.Content.ReadAsStringAsync();
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html); 

        var articles = htmlDoc.DocumentNode.SelectNodes("//article");

        var newsCollection = await CreateNewsObject(articles,SubjectEnum.Politics);

        return newsCollection;
    }
    
    private async Task<News[]> GetPolitcsHealth()
    {
        var response = await httpclient.GetAsync("https://jornalbeirario.com.br/portal/?cat=18");
        var html = await response.Content.ReadAsStringAsync();
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html); 

        var articles = htmlDoc.DocumentNode.SelectNodes("//article");
        var newsCollection = await CreateNewsObject(articles,SubjectEnum.Health);

        return newsCollection;

    }

    private Task<News[]> CreateNewsObject(HtmlNodeCollection articles, SubjectEnum subject)
    {
        var newsCollection = new List<News>();

        foreach (var article in articles)
        {
            var title = article.SelectSingleNode(".//h2").InnerText;
            var link = article.SelectSingleNode(".//span//a").GetAttributeValue("href", ""); 
            var date = article.SelectSingleNode(".//span//a//time").GetAttributeValue("datetime", ""); 
            var content = article.SelectSingleNode(".//p").InnerText;

            var image = article.SelectSingleNode(".//img")?.GetAttributeValue("src", "") ?? null; 
            
            title = HtmlEntity.DeEntitize(title);
            content = HtmlEntity.DeEntitize(content);

            var news = new News(
                title.Trim(),
                content.Trim(),
                "Jornal Beira Rio",
                subject,
                DateTime.Parse(date),
                new Uri(link),
                image is null ? null : new Uri(image)
            );

            newsCollection.Add(news);
        }

        return Task.FromResult(newsCollection.ToArray());
    }
}
