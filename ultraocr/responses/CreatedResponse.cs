// <copyright file="CreatedResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Created response.
/// </summary>
public class CreatedResponse
{
    /// <summary>
    /// Gets or sets the Status Url.
    /// </summary>
    [JsonPropertyName("status_url")]
    public required string StatusUrl { get; set; }

    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }
}