// <copyright file="SignedUrlResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Signed url response.
/// </summary>
public class SignedUrlResponse
{
    /// <summary>
    /// Gets or sets the Status url.
    /// </summary>
    [JsonPropertyName("status_url")]
    public required string StatusUrl { get; set; }

    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the Exp.
    /// </summary>
    [JsonPropertyName("exp")]
    public required long Exp { get; set; }

    /// <summary>
    /// Gets or sets the Urls.
    /// </summary>
    [JsonPropertyName("urls")]
    public required Dictionary<string, string> Urls { get; set; }
}