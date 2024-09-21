using HtmlAgilityPack;


namespace Core.News.DataSources;

public class DiarioDoVale(HttpClient httpclient)
{
    public async Task<News[]> GetNews()
    {
        var news = new List<News>();
        news.AddRange(await GetPoliticsNews());

        return news.ToArray();
    }

    private async Task<News[]> GetPoliticsNews()
    {
        var response = await httpclient.GetAsync("https://diariodovale.com.br/editoria/politica/");
        var html = await response.Content.ReadAsStringAsync();
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html); 

        var articles = htmlDoc.DocumentNode.SelectNodes("//article");

        var newsCollections = await CreateNewsObject(articles, SubjectEnum.Politics);
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
            var content = article.SelectSingleNode(".//p").InnerText;

            var image = article.SelectSingleNode("//a[contains(@class, 'penci-image-holder')]")?.GetAttributeValue("data-bgset", null);


            var news = new News() 
            {
                Title = title.Trim(),
                OriginalUrl = new Uri(link),
                PublishedAt = DateTime.Parse(date),
                Content = content,
                ImageUrl = image is null ? null : new Uri(image), 
                Author = "Diario do Vale",
                Subject = subject
            };

            newsCollection.Add(news);
        }

        return Task.FromResult(newsCollection.ToArray());
    }
}