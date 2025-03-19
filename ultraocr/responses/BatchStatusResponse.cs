namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class BatchStatusResponse
{
    [JsonPropertyName("batch_ksuid")]
    public required string BatchKsuid { get; set; }

    [JsonPropertyName("created_at")]
    public required string CreatedAt { get; set; }

    [JsonPropertyName("service")]
    public required string Service { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("jobs")]
    public List<BatchStatusJobs>? Jobs { get; set; }
}