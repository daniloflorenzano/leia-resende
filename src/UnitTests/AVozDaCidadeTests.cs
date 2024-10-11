using Core.News.DataSources;
using Xunit.Abstractions;

namespace UnitTests;

public class AVozDaCidadeTests(ITestOutputHelper output)
{
    [Fact]
    public async Task GetNews()
    {   
        var httpClient = new HttpClient();
        var aVozDaCidade = new AVozDaCidade(httpClient);
        var news = await aVozDaCidade.GetNews();
        Assert.True(news.Length > 0);

        foreach (var n in news)
        {
            output.WriteLine(n.ToString());
            output.WriteLine("====================================");
        }
    }
}