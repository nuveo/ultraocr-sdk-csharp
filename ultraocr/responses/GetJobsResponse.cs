namespace ultraocr.Responses;

using System.Text.Json.Serialization;

public class GetJobsResponse
{
    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; set; }

    [JsonPropertyName("jobs")]
    public required List<JobResultResponse> Jobs { get; set; }
}