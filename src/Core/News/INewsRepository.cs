using Core.Application;

namespace Core.News;

public interface INewsRepository
{
    Task Write(List<News> news);
    Task<List<News>> Read(SearchFilter? filter = null);
    Task<int> Count();
    Task<List<SubjectEnum>> GetAvailableSubjects();
}
