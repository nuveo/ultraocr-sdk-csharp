// <copyright file="JobTimeoutException.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr.Exceptions;

/// <summary>
/// Exception when job reach the timeout.
/// </summary>
public class JobTimeoutException(long timeout) : Exception($"timeout reached after {timeout} seconds")
{
    /// <summary>
    /// Gets the Timeout.
    /// </summary>
    public long Timeout { get; } = timeout;
}
