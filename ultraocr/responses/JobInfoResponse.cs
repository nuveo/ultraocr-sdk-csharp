// <copyright file="JobInfoResponse.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Job info response.
/// </summary>
public class JobInfoResponse
{
    /// <summary>
    /// Gets or sets the Batch id.
    /// </summary>
    [JsonPropertyName("job_id")]
    public required string JobId { get; set; }

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
    public required string? ValidationId { get; set; }

    /// <summary>
    /// Gets or sets the Created At.
    /// </summary>
    [JsonPropertyName("created_at")]
    public required string CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the Finished At.
    /// </summary>
    [JsonPropertyName("finished_at")]
    public string? FinishedAt { get; set; }

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
    /// Gets or sets the Validation status.
    /// </summary>
    [JsonPropertyName("validation_status")]
    public string? ValidationStatus { get; set; }

    /// <summary>
    /// Gets or sets the Client data.
    /// </summary>
    [JsonPropertyName("client_data")]
    public object? ClientData { get; set; }

    /// <summary>
    /// Gets or sets the Result.
    /// </summary>
    [JsonPropertyName("result")]
    public Result? Result { get; set; }

    /// <summary>
    /// Gets or sets the Validation.
    /// </summary>
    [JsonPropertyName("validation")]
    public object? Validation { get; set; }

    /// <summary>
    /// Gets or sets the Metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}