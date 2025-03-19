namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class JobResultResponse
{
    [JsonPropertyName("job_ksuid")]
    public required string JobKsuid { get; set; }

    [JsonPropertyName("created_at")]
    public required string CreatedAt { get; set; }

    [JsonPropertyName("service")]
    public required string Service { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("process_time")]
    public string? ProcessTime { get; set; }

    [JsonPropertyName("validation_status")]
    public string? ValidationStatus { get; set; }

    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    [JsonPropertyName("client_data")]
    public object? ClientData { get; set; }

    [JsonPropertyName("validation")]
    public object? Validation { get; set; }

    [JsonPropertyName("result")]
    public Result? Result { get; set; }

    [JsonPropertyName("jobs")]
    public List<BatchStatusJobs>? Jobs { get; set; }
}