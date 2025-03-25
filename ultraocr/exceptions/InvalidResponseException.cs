// <copyright file="InvalidResponseException.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr.Exceptions;

/// <summary>
/// Exception when response is null.
/// </summary>
public class InvalidResponseException() : Exception("invalid response")
{
}
