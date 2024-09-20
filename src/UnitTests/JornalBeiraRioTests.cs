using Core.News.DataSources;
using Xunit.Abstractions;

namespace UnitTests;

public class JornalBeiraRioTests(ITestOutputHelper output)
{

    [Fact]
    public async Task GetPolitcsNews()
    {   
        var httpClient = new HttpClient();
        var jornalBeiraRio = new JornalBeiraRio(httpClient);
        var news = await jornalBeiraRio.GetNews();
        Assert.True(news.Length > 0);

        foreach (var n in news)
        {
            output.WriteLine(n.ToString());
            output.WriteLine("====================================");
        }
    }
}