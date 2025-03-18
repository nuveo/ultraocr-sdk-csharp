namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class TokenResponse
{
    [JsonPropertyName("token")]
    public required string Token { get; set; }
}
