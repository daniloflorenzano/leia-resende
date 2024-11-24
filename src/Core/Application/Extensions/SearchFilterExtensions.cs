using System.Linq.Expressions;

namespace Core.Application.Extensions;

public static class SearchFilterExtensions
{
    public static SearchFilter WithWhere(this SearchFilter filter, Expression<Func<News.News, bool>> where)
    {
        filter.Where = where ?? throw new ArgumentNullException(nameof(where));
        return filter;
    }
    
    public static SearchFilter WithOrderBy(this SearchFilter filter, Expression<Func<News.News, object>> orderBy)
    {
        filter.OrderBy = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
        return filter;
    }
    
    public static SearchFilter WithOrderByDesc(this SearchFilter filter, Expression<Func<News.News, object>> orderBy)
    {
        filter.WithOrderBy(orderBy);
        filter.OrderByDescending = true;
        return filter;
    }
    
    public static SearchFilter WithPagination(this SearchFilter filter, int start, int end)
    {
        if (start < 0 || end < 0 || start > end)
        {
            throw new ArgumentException("Invalid pagination range");
        }
        filter.PaginationStart = start;
        filter.PaginationEnd = end;
        return filter;
    }
}