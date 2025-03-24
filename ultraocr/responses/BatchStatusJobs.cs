// <copyright file="BatchStatusJobs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The batch status jobs response.
/// </summary>
public class BatchStatusJobs
{
    /// <summary>
    /// Gets or sets the Job ksuid.
    /// </summary>
    [JsonPropertyName("job_ksuid")]
    public required string JobKsuid { get; set; }

    /// <summary>
    /// Gets or sets the Created at.
    /// </summary>
    [JsonPropertyName("created_at")]
    public required string CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the Result url.
    /// </summary>
    [JsonPropertyName("result_url")]
    public required string ResultUrl { get; set; }

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
}