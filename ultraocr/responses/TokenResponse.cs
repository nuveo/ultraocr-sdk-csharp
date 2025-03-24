// <copyright file="TokenResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Token response.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// Gets or sets the Token.
    /// </summary>
    [JsonPropertyName("token")]
    public required string Token { get; set; }
}
