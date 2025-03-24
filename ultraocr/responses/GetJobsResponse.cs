// <copyright file="GetJobsResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Get jobs response.
/// </summary>
public class GetJobsResponse
{
    /// <summary>
    /// Gets or sets the NextPageToken.
    /// </summary>
    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; set; }

    /// <summary>
    /// Gets or sets the Jobs.
    /// </summary>
    [JsonPropertyName("jobs")]
    public required List<JobResultResponse> Jobs { get; set; }
}