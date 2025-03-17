namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class SignedUrlResponse
{
    [JsonPropertyName("status_url")]
    public required string StatusUrl { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("exp")]
    public required long Exp { get; set; }

    [JsonPropertyName("urls")]
    public required Dictionary<string, string> Urls { get; set; }
}