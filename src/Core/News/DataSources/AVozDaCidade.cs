using HtmlAgilityPack;

namespace Core.News.DataSources;

public class AVozDaCidade(HttpClient httpclient) : IDataSource
{
    private const string SourceName = "A Voz da Cidade";
    private const string SourceIconUrl = "https://avozdacidade.com/wp/wp-content/uploads/2022/08/LOGO-A-VOZ-DA-CIDADE.jpg";
    
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
                    content += HtmlEntity.DeEntitize(p.InnerText) + "\n"; // Adiciona quebra de linha entre parágrafos
                }
            }

            var image = article.SelectSingleNode(".//div//a")?.GetAttributeValue("data-bgset", "") ?? null;

            title = HtmlEntity.DeEntitize(title);
            content = HtmlEntity.DeEntitize(content);

            var news = new News(
                title.Trim(),
                content.Trim(),
                SourceName,
                new Uri(SourceIconUrl),
                subject,
                DateTime.Parse(date),
                new Uri(link),
                image is null ? null : new Uri(image)
            );
            
            if (news.Content.StartsWith("Resende", StringComparison.InvariantCultureIgnoreCase) || news.Content.StartsWith("Sul Fluminense", StringComparison.InvariantCultureIgnoreCase))
                newsCollection.Add(news);
        }

        return Task.FromResult(newsCollection.ToArray());
    }
}