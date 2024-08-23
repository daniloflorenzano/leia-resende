using System.Linq.Expressions;

namespace Core.News;

public interface INewsRepository
{
    Task Create(News news);
    Task<IEnumerable<News>> Read(Expression<Func<News, bool>>? where = null);
}
