using Core.News.DataSources;
using Xunit.Abstractions;

namespace UnitTests;

public class PortalDaCidadeTests(ITestOutputHelper output)
{

    [Fact]
    public async Task GetNews()
    {   
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:132.0) Gecko/20100101 Firefox/132.0");
        var jornalBeiraRio = new PortalDaCidade(httpClient);
        var news = await jornalBeiraRio.GetNews();
        Assert.True(news.Length > 0);

        foreach (var n in news)
        {
            output.WriteLine(n.ToString());
            output.WriteLine("====================================");
        }
    }
}