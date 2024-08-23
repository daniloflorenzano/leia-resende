namespace Core.News;

public sealed class News
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public SubjectEnum Subject { get; set; }
    public DateTime PublishedAt { get; set; }
    public Uri OriginalUrl { get; set; } = null!;
    public Uri ImageUrl { get; set; } = null!;
}
