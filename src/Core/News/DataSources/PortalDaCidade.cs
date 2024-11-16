using Core.Extensions;
using HtmlAgilityPack;

namespace Core.News.DataSources;

public class PortalDaCidade(HttpClient httpClient) : IDataSource
{
    private const string SourceName = "Portal da Cidade";
    private const string SourceIconUrl = "https://resende.portaldacidade.com/static/favicon.ico";
    
    public async Task<News[]> GetNews()
    {
        var news = new List<News>();
        news.AddRange(await GetNewsBySubject("cultura", SubjectEnum.Culture));
        news.AddRange(await GetNewsBySubject("economia", SubjectEnum.Economy));
        news.AddRange(await GetNewsBySubject("educacao", SubjectEnum.Education));
        news.AddRange(await GetNewsBySubject("esportes", SubjectEnum.Sports));
        news.AddRange(await GetNewsBySubject("politica", SubjectEnum.Politics));
        news.AddRange(await GetNewsBySubject("saude", SubjectEnum.Health));
        news.AddRange(await GetNewsBySubject("turismo", SubjectEnum.Tourism));
        
        // generico, podem vir noticias repetidas
        news.AddRange(await GetNewsBySubject("cidade", SubjectEnum.Undefined));
        return news.ToArray();
    }

    private async Task<News[]> GetNewsBySubject(string editoriaId, SubjectEnum subjectEnum)
    {
        try
        {
            var response = await httpClient.GetAsync($"https://resende.portaldacidade.com/noticias/{editoriaId}");
            var html = await response.Content.ReadAsStringAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var newsNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='news-flex']");
            var linkNodes = newsNode.SelectNodes(".//a");

            var newsCollections = CreateNewsObject(linkNodes, subjectEnum);
            return newsCollections;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao capturar notícias do site Portal da Cidade para o assunto {editoriaId}: {e.Message}");
            return [];
        }
    }
    
    private News[] CreateNewsObject(HtmlNodeCollection linkNodes, SubjectEnum subject)
    {
        var newsCollection = new List<News>();
        
        foreach (var article in linkNodes)
        {
            var title = article.SelectSingleNode(".//h2").InnerText;
            var link = article.GetAttributeValue("href", "");
            var date = article.SelectSingleNode("//div[2]/p[3]").InnerText;
            var content = article.SelectSingleNode("//div[2]/p[2]").InnerText;
            var image = article.SelectSingleNode(".//img").GetAttributeValue("src", "");
            
            title = HtmlEntity.DeEntitize(title);
            content = HtmlEntity.DeEntitize(content);
            date = HtmlEntity.DeEntitize(date);
            
            date = date.ReturnStringBetween("Publicado em ", " às");
            
            var news = new News(
                title,
                content,
                SourceName,
                new Uri(SourceIconUrl),
                subject,
                DateTime.Parse(date),
                new Uri(link),
                new Uri(image));
            newsCollection.Add(news);
        }
        
        return newsCollection.ToArray();
    }
}