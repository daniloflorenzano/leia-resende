using Microsoft.Extensions.Logging;

namespace Core.News;

public sealed class RegisterNews(ILogger<RegisterNews> logger, INewsRepository newsRepository) : IObserver<News[]>
{

    public async Task Handle(News[] news){
        //logger.LogInformation("Registrando notícia: {0}", news.Title);

        //ValidateNews(news);
        await newsRepository.CreateRange(news);
    }

    private void ValidateNews(News news){
        //TODO: definir validacoes
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public async void OnNext(News[] value)
    {
        await Handle(value);
    }
}
