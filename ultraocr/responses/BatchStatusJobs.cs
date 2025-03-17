namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class BatchStatusJobs
{
    [JsonPropertyName("job_ksuid")]
    public required string JobKsuid { get; set; }

    [JsonPropertyName("created_at")]
    public required string CreatedAt { get; set; }

    [JsonPropertyName("result_url")]
    public required string ResultUrl { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}