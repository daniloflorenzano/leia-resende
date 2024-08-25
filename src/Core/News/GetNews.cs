using Microsoft.Extensions.Logging;

namespace Core.News;

public sealed class GetNews(ILogger<GetNews> logger, INewsRepository newsRepository)
{
    public async Task<IEnumerable<News>> Handle(SearchFilter? filter = null)
    {     
        logger.LogInformation("Buscando notícias");

        if (filter?.Subjects is not null) {

            if (!string.IsNullOrEmpty(filter.NewsTitle)) {
                return await newsRepository.Read(
                    news => 
                        filter.Subjects.Contains(news.Subject) &&
                        news.Title.Contains(filter.NewsTitle)
                );
            }
            
            return await newsRepository.Read(news => filter.Subjects.Contains(news.Subject));
        }

        return await newsRepository.Read();
    }
}

public sealed class SearchFilter
{
    public string NewsTitle { get; private set; } = string.Empty;
    public SubjectEnum[]? Subjects { get; private set; }

    public void SetTitle(string title)
    {
        ValidateTitle(title);
        NewsTitle = title;
    }

    private void ValidateTitle(string title) {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty");

        if (title.Length > 50)
            throw new ArgumentException("Title cannot be greater than 50 characters");
    }

    public void SetSubjects(params SubjectEnum[] subjects)
    {
        Subjects = subjects;
    }
}
