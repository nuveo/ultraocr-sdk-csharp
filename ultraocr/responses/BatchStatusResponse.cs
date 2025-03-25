// <copyright file="BatchStatusResponse.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Batch status response.
/// </summary>
public class BatchStatusResponse
{
    /// <summary>
    /// Gets or sets the Batch ksuid.
    /// </summary>
    [JsonPropertyName("batch_ksuid")]
    public required string BatchKsuid { get; set; }

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
    /// Gets or sets the Jobs.
    /// </summary>
    [JsonPropertyName("jobs")]
    public List<BatchStatusJobs>? Jobs { get; set; }
}