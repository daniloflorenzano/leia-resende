using Core.News.DataSources;
using Xunit.Abstractions;

namespace UnitTests;

public class DiarioDoValeTests(ITestOutputHelper output)
{
    [Fact]
    public async Task GetNews()
    {   
        var httpClient = new HttpClient();
        var diarioDoVale = new DiarioDoVale(httpClient);
        var news = await diarioDoVale.GetNews();
        Assert.True(news.Length > 0);

        foreach (var n in news)
        {
            output.WriteLine(n.ToString());
            output.WriteLine("====================================");
        }
    }
}