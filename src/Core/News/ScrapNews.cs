using Core.News.DataSources;

namespace Core.News;

public sealed class ScrapNews : IObservable<News[]>
{
    #region Singleton Pattern
    private static ScrapNews? _instance;

    public static ScrapNews GetInstance() => _instance ??= new ScrapNews();

    private ScrapNews()
    {
        observers = new List<IObserver<News[]>>();
    }
    #endregion

    #region Observer Pattern
    private List<IObserver<News[]>> observers;

    private void NotifySubs(News[] news)
    {
        foreach (var observer in observers)
            observer.OnNext(news);
    }

    public IDisposable Subscribe(IObserver<News[]> observer)
    {
        if (! observers.Contains(observer))
            observers.Add(observer);

        return new Unsubscriber(observers, observer);
    }


    private class Unsubscriber : IDisposable
    {
        private List<IObserver<News[]>> _observers;
        private IObserver<News[]> _observer;

        public Unsubscriber(List<IObserver<News[]>> observers, IObserver<News[]> observer)
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

    public async Task Handle()
    {
        var httpClient = new HttpClient();
        var diarioDoVale = new DiarioDoVale(httpClient);
        var jornalBeiraRio = new JornalBeiraRio(httpClient);

        try {
            var news = await diarioDoVale.GetNews();
            NotifySubs(news);

            news = await jornalBeiraRio.GetNews();
            NotifySubs(news);
        }
        catch (Exception)
        {
            // nao faz nada
            return;
        }
    }    
}


