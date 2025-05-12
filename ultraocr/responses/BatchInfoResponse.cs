// <copyright file="BatchInfoResponse.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Batch info response.
/// </summary>
public class BatchInfoResponse
{
    /// <summary>
    /// Gets or sets the Batch id.
    /// </summary>
    [JsonPropertyName("batch_id")]
    public required string BatchId { get; set; }

    /// <summary>
    /// Gets or sets the Client id.
    /// </summary>
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the Company id.
    /// </summary>
    [JsonPropertyName("company_id")]
    public required string CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the Validation id.
    /// </summary>
    [JsonPropertyName("validation_id")]
    public string? ValidationId { get; set; }

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
    /// Gets or sets the Source.
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; set; }

    /// <summary>
    /// Gets or sets the Total jobs.
    /// </summary>
    [JsonPropertyName("total_jobs")]
    public int? TotalJobs { get; set; }

    /// <summary>
    /// Gets or sets the Total processed.
    /// </summary>
    [JsonPropertyName("total_processed")]
    public int? TotalProcessed { get; set; }
}