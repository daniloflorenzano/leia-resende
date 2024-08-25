namespace Core.NewsSearchs;

public interface INewsSearchRepository
{
    Task Create(NewsSearch newsSearch);
}
