using Core.Application;
using Microsoft.Extensions.Logging;

namespace Core.News;

public sealed class RegisterNews(ILogger<RegisterNews> logger, INewsRepository newsRepository) : IObserver<List<News>>
{
    private async Task Handle(List<News> news){
        news = RemoveDuplicatedNews(news);   
        await newsRepository.Write(news);
    }
    
    private List<News> RemoveDuplicatedNews(List<News> news){
        return news.GroupBy(n => new {n.Title, n.PublishedAt}).Select(g => g.First()).ToList();
    }
    
    public void OnCompleted()
    {
        var globalInfo = GlobalInfo.GetInstance();
        globalInfo.TotalNews = newsRepository.Count().Result;
        globalInfo.AvailableSubjects = newsRepository.GetAvailableSubjects().Result;
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(List<News> value)
    {
        Handle(value).Wait();
    }
}
