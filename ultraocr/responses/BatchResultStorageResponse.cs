// <copyright file="BatchResultStorageResponse.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Batch result on storage response.
/// </summary>
public class BatchResultStorageResponse
{
    /// <summary>
    /// Gets or sets the Url.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    /// <summary>
    /// Gets or sets the Exp.
    /// </summary>
    [JsonPropertyName("exp")]
    public required string Exp { get; set; }
}