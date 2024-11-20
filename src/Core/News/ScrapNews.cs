using System.Reflection;
using Core.News.DataSources;

namespace Core.News;

public sealed class ScrapNews : IObservable<List<News>>
{
    #region Singleton Pattern

    private static ScrapNews? _instance;

    public static ScrapNews GetInstance() => _instance ??= new ScrapNews();

    private ScrapNews()
    {
        _observers = [];
    }

    #endregion

    #region Observer Pattern

    private readonly List<IObserver<List<News>>> _observers;

    private void NotifySubs(List<News> news)
    {
        foreach (var observer in _observers)
            observer.OnNext(news);
    }

    public IDisposable Subscribe(IObserver<List<News>> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);

        return new Unsubscriber(_observers, observer);
    }


    private class Unsubscriber : IDisposable
    {
        private List<IObserver<List<News>>> _observers;
        private IObserver<List<News>> _observer;

        public Unsubscriber(List<IObserver<List<News>>> observers, IObserver<List<News>> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            // ReSharper disable once NegativeEqualityExpression
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (!(_observer == null))
                _observers.Remove(_observer);
        }
    }

    #endregion

    public async Task Handle()
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (X11; Linux x86_64; rv:132.0) Gecko/20100101 Firefox/132.0");


            // usa reflection para pegar todas as classes que herdam de IDataSource
            // e chama o método GetNews
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IDataSource)))
                .ToList();

            // Configurando o máximo de paralelismo
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            var newsList = new List<News>();

            await Parallel.ForEachAsync(types, options, async (type, _) =>
            {
                var instance = Activator.CreateInstance(type, httpClient);
                var method = type.GetMethod("GetNews");
                var news = await (Task<News[]>)method!.Invoke(instance, null)!;
                newsList.AddRange(news);
            });

            NotifySubs(newsList);
        }
    }
}