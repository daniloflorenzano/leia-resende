using System.Text.Encodings.Web;
using System.Text.Json;

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
    public Uri? ImageUrl { get; set; } = null!;

    public override string ToString()
    {
        var json = JsonSerializer.Serialize(this,
            new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        
        return json;
    }
}
