using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.News;

public sealed class News
{
    
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;
    
    public SubjectEnum Subject { get; set; }

    public DateTime PublishedAt { get; set; }

    public Uri OriginalUrl { get; set; } = null!;

    public Uri? ImageUrl { get; set; } = null!;

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

    public string GetFormatedContent() {
        return Content.Length > 200 ? Content.Substring(0, 200) + "..." : Content;
    }
}
