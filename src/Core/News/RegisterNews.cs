namespace Core.News;

public sealed class RegisterNews(INewsRepository newsRepository) : IObserver<News>
{

    public async Task Handle(News news){
        ValidateNews(news);
        await newsRepository.Create(news);
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

    public void OnNext(News value)
    {
        throw new NotImplementedException();
    }
}
