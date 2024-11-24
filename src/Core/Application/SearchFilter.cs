using System.Linq.Expressions;

namespace Core.Application;

public sealed class SearchFilter
{
    public int PaginationStart { get; set; }
    public int PaginationEnd { get; set; }
    public Expression<Func<News.News, bool>>? Where { get; set; }
    public Expression<Func<News.News, object>>? OrderBy { get; set; }
    public bool OrderByDescending { get; set; }
}