namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class Result
{
    [JsonPropertyName("Document")]
    public object? Document { get; set; }

    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("Time")]
    public string? Time { get; set; }
}