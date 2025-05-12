// <copyright file="Constants.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr;

/// <summary>
/// The system constants.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The status OK.
    /// </summary>
    public const int STATUS_OK = 200;

    /// <summary>
    /// Pooling interval.
    /// </summary>
    public const int POOLING_INTERVAL = 1;

    /// <summary>
    /// Api timeout.
    /// </summary>
    public const int API_TIMEOUT = 30;

    /// <summary>
    /// Token expiration time.
    /// </summary>
    public const int DEFAULT_EXPIRATION_TIME = 60;

    /// <summary>
    /// Base URL.
    /// </summary>
    public const string BASE_URL = "https://ultraocr.apis.nuveo.ai/v2";

    /// <summary>
    /// Auth base URL.
    /// </summary>
    public const string AUTH_BASE_URL = "https://auth.apis.nuveo.ai/v2";

    /// <summary>
    /// Status done.
    /// </summary>
    public const string STATUS_DONE = "done";

    /// <summary>
    /// Status error.
    /// </summary>
    public const string STATUS_ERROR = "error";

    /// <summary>
    /// Application json.
    /// </summary>
    public const string APPLICATION_JSON = "application/json";

    /// <summary>
    /// Header accept.
    /// </summary>
    public const string HEADER_ACCEPT = "Accept";

    /// <summary>
    /// Content type.
    /// </summary>
    public const string HEADER_CONTENT_TYPE = "Content-Type";

    /// <summary>
    /// Authorization.
    /// </summary>
    public const string HEADER_AUTHORIZATION = "Authorization";

    /// <summary>
    /// Bearer prefix.
    /// </summary>
    public const string BEARER_PREFIX = "Bearer ";

    /// <summary>
    /// Document key.
    /// </summary>
    public const string KEY_DOCUMENT = "document";

    /// <summary>
    /// Facematch key.
    /// </summary>
    public const string KEY_FACEMATCH = "facematch";

    /// <summary>
    /// Extra key.
    /// </summary>
    public const string KEY_EXTRA = "extra-document";

    /// <summary>
    /// Selfie key.
    /// </summary>
    public const string KEY_SELFIE = "selfie";

    /// <summary>
    /// Extra url key.
    /// </summary>
    public const string KEY_EXTRA_URL = "extra_document";

    /// <summary>
    /// Flag true.
    /// </summary>
    public const string FLAG_TRUE = "true";

    /// <summary>
    /// Base64 attribute.
    /// </summary>
    public const string BASE64_ATTRIBUTE = "base64";

    /// <summary>
    /// Return attribute.
    /// </summary>
    public const string RETURN_ATTRIBUTE = "return";

    /// <summary>
    /// Return request.
    /// </summary>
    public const string RETURN_REQUEST = "request";

    /// <summary>
    /// Return storage.
    /// </summary>
    public const string RETURN_STORAGE = "storage";
}
