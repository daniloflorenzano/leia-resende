using System.Linq.Expressions;

namespace Core.News;

public interface INewsRepository
{
    Task Write(List<News> news);
    Task<List<News>> Read(Expression<Func<News, bool>>? where = null);
}
