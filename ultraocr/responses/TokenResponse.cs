namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class TokenResponse
{
    [JsonPropertyName("Token")]
    public required string Token { get; set; }
}
