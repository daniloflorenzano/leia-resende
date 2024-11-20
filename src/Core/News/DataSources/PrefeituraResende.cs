using System.Web;
using HtmlAgilityPack;

namespace Core.News.DataSources;

public class PrefeituraResende(HttpClient httpClient) : IDataSource
{
    private const string SourceName = "Prefeitura de Resende";
    private const string SourceIconUrl = "https://resende.rj.gov.br/images/logo.WebP";

    public Task<News[]> GetNews() 
        => GetNewsOfCurrentDay();

    private async Task<News[]> GetNewsOfCurrentDay()
    {
        try
        {
            var dateFilter = DateTime.Now.ToString("dd/MM/yyyy");
            var dateFilterEncoded = HttpUtility.UrlEncode(dateFilter);

            var response =
                await httpClient.GetAsync($"https://resende.rj.gov.br/noticias?de={dateFilterEncoded}&page=1");
            var html = await response.Content.ReadAsStringAsync();

            if (html.Contains("Nenhuma notícia encontrada."))
                return [];
            
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var pagination = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='pagination']");
            if (pagination is null)
            {
                var newsNodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='item']");
                return CreateNewsObject(newsNodes);
            }
            
            var lastPage = pagination.SelectSingleNode(".//a[last()-1]").InnerText;

            if (!int.TryParse(lastPage, out var totalPages))
                return [];

            var news = new List<News>();
            for (var i = 1; i <= totalPages; i++)
            {
                if (i > 1)
                {
                    var responsePage =
                        await httpClient.GetAsync(
                            $"https://resende.rj.gov.br/noticias?de={dateFilterEncoded}&page={i}");
                    html = await responsePage.Content.ReadAsStringAsync();
                }

                var htmlDocPage = new HtmlDocument();
                htmlDocPage.LoadHtml(html);
                var newsNodes = htmlDocPage.DocumentNode.SelectNodes("//a[@class='item']");
                var newsCollections = CreateNewsObject(newsNodes);
                news.AddRange(newsCollections);
            }

            return news.ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao capturar notícias do site Prefeitura de Resende: {e.Message}");
            return [];
        }

    }

    private News[] CreateNewsObject(HtmlNodeCollection newsNodes)
    {
        var newsCollection = new List<News>();
        
        foreach (var node in newsNodes)
        {
            var title = node.SelectSingleNode(".//p[@class='title']").InnerText;
            var link = node.GetAttributeValue("href", "");
            var date = node.SelectSingleNode(".//p[@class='date']").InnerText;
            var image = "https://resende.rj.gov.br" + node.SelectSingleNode(".//img").GetAttributeValue("src", "");
            
            title = HtmlEntity.DeEntitize(title);
            date = HtmlEntity.DeEntitize(date);
            
            var news = new News(
                title.Trim(),
                string.Empty,
                SourceName,
                new Uri(SourceIconUrl),
                SubjectEnum.Undefined,
                DateTime.Parse(date),
                new Uri(link),
                new Uri(image)
            );
            
            newsCollection.Add(news);
        }
        
        return newsCollection.ToArray();
    }
}