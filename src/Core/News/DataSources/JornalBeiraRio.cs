using HtmlAgilityPack;

namespace Core.News.DataSources;

public class JornalBeiraRio(HttpClient httpclient)
{
    public async Task<News[]> GetNews(){
        var news = new List<News>();
        news.AddRange(await GetPolitcsNews());
    
        return news.ToArray();
    }

    private async Task<News[]> GetPolitcsNews()
    {
        var response = await httpclient.GetAsync("https://jornalbeirario.com.br/portal/?cat=11");
        var html = await response.Content.ReadAsStringAsync();
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html); 

        var articles = htmlDoc.DocumentNode.SelectNodes("//article");

        var newsCollection = new List<News>();

        foreach (var article in articles)
        {
            var title = article.SelectSingleNode(".//h2").InnerText;
            var link = article.SelectSingleNode(".//span//a").GetAttributeValue("href", ""); 
            var date = article.SelectSingleNode(".//span//a//time").GetAttributeValue("datetime", ""); 
            var content = article.SelectSingleNode(".//p").InnerText;

            var image = article.SelectSingleNode(".//img")?.GetAttributeValue("src", "") ?? null; 
            
            var news = new News() 
            {
                Title = title.Trim(),
                OriginalUrl = new Uri(link),
                PublishedAt = DateTime.Parse(date),
                Content = content,
                ImageUrl = image is null ? null : new Uri(image),
                Author = "Jornal Beira Rio",
                Subject = SubjectEnum.Politics
            };

            newsCollection.Add(news);
        }

        return newsCollection.ToArray();
    }
}
