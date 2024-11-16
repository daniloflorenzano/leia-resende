using HtmlAgilityPack;

namespace Core.News.DataSources;

public class JornalBeiraRio(HttpClient httpclient) : IDataSource
{
    private const string SourceName = "Jornal Beira Rio";
    private const string SourceIconUrl = "https://jornalbeirario.com.br/portal/wp-content/uploads/2017/09/beira_rio_icon-150x150.png";
    
    public async Task<News[]> GetNews(){
        var news = new List<News>();
        news.AddRange(await GetNewsBySubject("11", SubjectEnum.Politics));
        news.AddRange(await GetNewsBySubject("18", SubjectEnum.Health));
        news.AddRange(await GetNewsBySubject("6", SubjectEnum.Economy));
        news.AddRange(await GetNewsBySubject("27", SubjectEnum.Environment));
        news.AddRange(await GetNewsBySubject("16", SubjectEnum.Environment));
    
        return news.ToArray();
    }

    private async Task<News[]> GetNewsBySubject(string catId, SubjectEnum subjectEnum)
    {
        try
        {
            var response = await httpclient.GetAsync($"https://jornalbeirario.com.br/portal/?cat={catId}");
            var html = await response.Content.ReadAsStringAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var articles = htmlDoc.DocumentNode.SelectNodes("//article");

            var newsCollection = CreateNewsObject(articles, SubjectEnum.Health);

            return newsCollection;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao capturar notícias do site Jornal Beira Rio para o assunto {catId}: {e.Message}");
            return [];
        }
    }

    private News[] CreateNewsObject(HtmlNodeCollection articles, SubjectEnum subject)
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
                SourceName,
                new Uri(SourceIconUrl),
                subject,
                DateTime.Parse(date),
                new Uri(link),
                image is null ? null : new Uri(image)
            );

            newsCollection.Add(news);
        }

        return newsCollection.ToArray();
    }
}
