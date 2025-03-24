// <copyright file="JobResultResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Job result response.
/// </summary>
public class JobResultResponse
{
    /// <summary>
    /// Gets or sets the Job ksuid.
    /// </summary>
    [JsonPropertyName("job_ksuid")]
    public required string JobKsuid { get; set; }

    /// <summary>
    /// Gets or sets the Created At.
    /// </summary>
    [JsonPropertyName("created_at")]
    public required string CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the Service.
    /// </summary>
    [JsonPropertyName("service")]
    public required string Service { get; set; }

    /// <summary>
    /// Gets or sets the Status.
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; set; }

    /// <summary>
    /// Gets or sets the Error.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the Process time.
    /// </summary>
    [JsonPropertyName("process_time")]
    public string? ProcessTime { get; set; }

    /// <summary>
    /// Gets or sets the Validation status.
    /// </summary>
    [JsonPropertyName("validation_status")]
    public string? ValidationStatus { get; set; }

    /// <summary>
    /// Gets or sets the Filename.
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>
    /// Gets or sets the Client data.
    /// </summary>
    [JsonPropertyName("client_data")]
    public object? ClientData { get; set; }

    /// <summary>
    /// Gets or sets the Validation.
    /// </summary>
    [JsonPropertyName("validation")]
    public object? Validation { get; set; }

    /// <summary>
    /// Gets or sets the Result.
    /// </summary>
    [JsonPropertyName("result")]
    public Result? Result { get; set; }

    /// <summary>
    /// Gets or sets the Jobs.
    /// </summary>
    [JsonPropertyName("jobs")]
    public List<BatchStatusJobs>? Jobs { get; set; }
}