using System.Reflection;
using Core.News.DataSources;

namespace Core.News;

public sealed class ScrapNews : IObservable<News>
{
    #region Singleton Pattern
    private static ScrapNews? _instance;

    public static ScrapNews GetInstance() => _instance ??= new ScrapNews();

    private ScrapNews()
    {
        observers = new List<IObserver<News>>();
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

    public async Task Handle()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:132.0) Gecko/20100101 Firefox/132.0");

        try {
            // usa reflection para pegar todas as classes que herdam de IDataSource
            // e chama o método GetNews
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IDataSource)))
                .ToList();

            // Configurando o máximo de paralelismo
            var options = new ParallelOptions { 
                MaxDegreeOfParallelism = Environment.ProcessorCount 
            };
            
            await Parallel.ForEachAsync(types, options, async (type, _) => {
                var instance = Activator.CreateInstance(type, httpClient);
                var method = type.GetMethod("GetNews");
                var news = await ((Task<News[]>) method!.Invoke(instance, null)!)!;
                foreach (var n in news)
                    NotifySubs(n);
            });
        }
        catch (Exception)
        {
            // nao faz nada
        }
    }    
}


