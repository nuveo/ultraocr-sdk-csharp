// <copyright file="Result.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// The Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets or sets the Document.
    /// </summary>
    [JsonPropertyName("Document")]
    public object? Document { get; set; }

    /// <summary>
    /// Gets or sets the Quantity.
    /// </summary>
    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the Time.
    /// </summary>
    [JsonPropertyName("Time")]
    public string? Time { get; set; }
}