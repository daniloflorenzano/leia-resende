using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.News;

public sealed class News
{
    public string Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;
    
    public Uri? AuthorIconUrl { get; set; }
    
    public SubjectEnum Subject { get; set; }

    public DateTime PublishedAt { get; set; }

    public Uri OriginalUrl { get; set; } = null!;

    public Uri? ImageUrl { get; set; } = null!;

    [Obsolete("Construtor para serialização", true)]
    public News()
    {
    }
    
    public News(string title, string content, string author, Uri authorIconUrl, SubjectEnum subject, DateTime publishedAt, Uri originalUrl, Uri? imageUrl)
    {
        Title = title;
        Content = content;
        Author = author;
        AuthorIconUrl = authorIconUrl;
        Subject = subject;
        PublishedAt = publishedAt;
        OriginalUrl = originalUrl;
        ImageUrl = imageUrl;
        
        Id = GenerateId();  
    }

    private string GenerateId()
    {
        var hash = new HashCode();
        hash.Add(Title);
        hash.Add(PublishedAt);
        hash.Add(OriginalUrl);
        return hash.ToHashCode().ToString();
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

    public string GetSubjectInPtBr()
    {
        return Subject switch
        {
            SubjectEnum.Sports => "Esportes",
            SubjectEnum.Politics => "Política",
            SubjectEnum.Economy => "Economia",
            SubjectEnum.Health => "Saúde",
            SubjectEnum.Woman => "Mulher",
            _ => "Outros"
        };
    }

    public string GetFormatedTitle() {
        return Title.Length > 100 ? Title.Substring(0, 97).Trim() + "..." : Title;
    }
}
