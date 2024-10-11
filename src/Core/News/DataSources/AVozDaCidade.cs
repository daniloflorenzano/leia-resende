using HtmlAgilityPack;

namespace Core.News.DataSources;

public class AVozDaCidade (HttpClient httpclient)
{
    public async Task<News[]> GetNews()
    {
        var news = new List<News>();
        news.AddRange(await GetEconomyNews());
        news.AddRange(await GetWomanNews());
        return news.ToArray();
    }

    private async Task<News[]> GetEconomyNews()
    {
        var response = await httpclient.GetAsync("https://avozdacidade.com/wp/editorias/economia/");
        var html = await response.Content.ReadAsStringAsync();
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html); 

        var articles = htmlDoc.DocumentNode.SelectNodes("//article");

        var newsCollections = await CreateNewsObject(articles, SubjectEnum.Economy);
        return newsCollections;
    }
    
    private async Task<News[]> GetWomanNews()
    {
        var response = await httpclient.GetAsync("https://avozdacidade.com/wp/editorias/mulher/");
        var html = await response.Content.ReadAsStringAsync();
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html); 

        var articles = htmlDoc.DocumentNode.SelectNodes("//article");

        var newsCollections = await CreateNewsObject(articles, SubjectEnum.Woman);
        return newsCollections;
    }

    private Task<News[]> CreateNewsObject(HtmlNodeCollection articles, SubjectEnum subject)
    {
        var newsCollection = new List<News>();

        foreach (var article in articles)
        {
            var title = article.SelectSingleNode(".//h2").InnerText;
            var link = article.SelectSingleNode(".//h2//a").GetAttributeValue("href", ""); 
            var date = article.SelectSingleNode(".//span//time").GetAttributeValue("datetime", ""); 
            var paragraphs = article.SelectNodes(".//p");
            var content = "";
    
            if (paragraphs != null)
            {
                foreach (var p in paragraphs)
                {
                    content += HtmlEntity.DeEntitize(p.InnerText) + "\n";  // Adiciona quebra de linha entre parágrafos
                }
            }
            var image = article.SelectSingleNode(".//div//a")?.GetAttributeValue("data-bgset", "") ?? null;

            title = HtmlEntity.DeEntitize(title);
            content = HtmlEntity.DeEntitize(content);

            var news = new News() 
            {
                Title = title.Trim(),
                OriginalUrl = new Uri(link),
                PublishedAt = DateTime.Parse(date),
                Content = content.Trim(),
                ImageUrl = image is null ? null : new Uri(image), 
                Author = "Diario do Vale",
                Subject = subject
            };

            newsCollection.Add(news);
        }

        return Task.FromResult(newsCollection.ToArray());
    }
}