namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class CreatedResponse
{
    [JsonPropertyName("status_url")]
    public required string StatusUrl { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }
}