using Microsoft.Extensions.Logging;

namespace Core.News;

public sealed class ScrapNews : IObservable<News>
{
    #region Singleton Pattern
    private static ScrapNews? _instance;

    public static ScrapNews GetInstance(ILogger<ScrapNews> logger) => _instance ??= new ScrapNews(logger);

    private ScrapNews(ILogger<ScrapNews> logger)
    {
        observers = new List<IObserver<News>>();
        this.logger = logger;
    }
    #endregion

    #region Observer Pattern
    private List<IObserver<News>> observers;

    private void NotifySubs(News news)
    {
        foreach (var observer in observers)
            observer.OnNext(news);
    }

    public IDisposable Subscribe(IObserver<News> observer)
    {
        if (! observers.Contains(observer))
            observers.Add(observer);

        return new Unsubscriber(observers, observer);
    }


    private class Unsubscriber : IDisposable
    {
        private List<IObserver<News>> _observers;
        private IObserver<News> _observer;

        public Unsubscriber(List<IObserver<News>> observers, IObserver<News> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (!(_observer == null)) 
                _observers.Remove(_observer);
        }
    }
    #endregion
    
    private readonly ILogger<ScrapNews> logger;

    public Task Handle()
    {
        logger.LogInformation("Buscando notícias");

        // TODO: criar uma cadeia de scrapers para cada site de notícias
        var dummyNews = new News();
        NotifySubs(dummyNews);
        return Task.CompletedTask;
    }    
}


