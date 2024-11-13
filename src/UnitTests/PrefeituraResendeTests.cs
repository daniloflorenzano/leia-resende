using Core.News.DataSources;
using Xunit.Abstractions;
using HttpClient = System.Net.Http.HttpClient;

namespace UnitTests;

public class PrefeituraResendeTests(ITestOutputHelper output)
{

    [Fact]
    public async Task GetNews()
    {   
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:132.0) Gecko/20100101 Firefox/132.0");
        var prefeituraResende = new PrefeituraResende(httpClient);
        var news = await prefeituraResende.GetNews();
        Assert.True(news.Length > 0);

        foreach (var n in news)
        {
            output.WriteLine(n.ToString());
            output.WriteLine("====================================");
        }
    }
}