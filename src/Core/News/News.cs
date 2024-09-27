using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.News;

public sealed class News
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "content")]
    public string Content { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "author")]
    public string Author { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "subject")]
    public SubjectEnum Subject { get; set; }

    [JsonProperty(PropertyName = "publishedAt")]
    public DateTime PublishedAt { get; set; }

    [JsonProperty(PropertyName = "originalUrl")]
    public Uri OriginalUrl { get; set; } = null!;

    [JsonProperty(PropertyName = "imageUrl")]
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
}
