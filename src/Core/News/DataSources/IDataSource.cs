namespace Core.News.DataSources;

public interface IDataSource
{
    Task<News[]> GetNews();
}