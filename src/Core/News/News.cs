using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.News;

public sealed class News
{
    public Guid Id { get; set; } = Guid.NewGuid(); 

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;
    
    public Uri? AuthorIconUrl { get; set; }
    
    public SubjectEnum Subject { get; set; }

    public DateTime PublishedAt { get; set; }

    public Uri OriginalUrl { get; set; } = null!;

    public Uri? ImageUrl { get; set; }

    [Obsolete("Construtor para serialização", true)]
    public News()
    {
    }
    
    public News(string title, string content, string author, Uri authorIconUrl, SubjectEnum subject, DateTime publishedAt, Uri originalUrl, Uri? imageUrl)
    {
        Title = title.Trim();
        Content = content.Trim();
        Author = author;
        AuthorIconUrl = authorIconUrl;
        Subject = subject;
        PublishedAt = publishedAt;
        OriginalUrl = originalUrl;
        ImageUrl = imageUrl;  
    }

    public override string ToString()
    {
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        };
        var json = JsonConvert.SerializeObject(this, settings);
        return json;
    }

    public string GetFormatedTitle() {
        return Title.Length > 100 ? Title.Substring(0, 97).Trim() + "..." : Title;
    }
}
