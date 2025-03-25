// <copyright file="Client.cs" company="Nuveo">
// Copyright (c) Nuveo. All rights reserved.
// </copyright>

namespace Ultraocr;

using System.Text.Json;
using System.Threading.Tasks;
using Ultraocr.Enums;
using Ultraocr.Exceptions;
using Ultraocr.Responses;

/// <summary>
/// Client to help on UltraOCR usage. For more details about all arguments and returns, access the oficial system documentation on https://docs.nuveo.ai/ocr/v2/.
/// </summary>
public class Client
{
    private string ClientID { get; set; }

    private string ClientSecret { get; set; }

    private string BaseUrl { get; set; }

    private string AuthBaseUrl { get; set; }

    private string Token { get; set; }

    private DateTime ExpiresAt { get; set; }

    private long Expires { get; set; }

    private long Timeout { get; set; }

    private int Interval { get; set; }

    private bool AutoRefresh { get; set; }

    private HttpClient RequestClient { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    public Client()
    {
        AuthBaseUrl = Constants.AUTH_BASE_URL;
        BaseUrl = Constants.BASE_URL;
        Timeout = Constants.API_TIMEOUT;
        Interval = Constants.POOLING_INTERVAL;
        Expires = Constants.DEFAULT_EXPIRATION_TIME;
        AutoRefresh = false;
        ClientID = string.Empty;
        ClientSecret = string.Empty;
        Token = string.Empty;
        ExpiresAt = DateTime.Now;
        RequestClient = new HttpClient();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="httpClient">The http client.</param>
    public Client(HttpClient httpClient)
    {
        AuthBaseUrl = Constants.AUTH_BASE_URL;
        BaseUrl = Constants.BASE_URL;
        Timeout = Constants.API_TIMEOUT;
        Interval = Constants.POOLING_INTERVAL;
        Expires = Constants.DEFAULT_EXPIRATION_TIME;
        ClientID = string.Empty;
        ClientSecret = string.Empty;
        Token = string.Empty;
        AutoRefresh = false;
        ExpiresAt = DateTime.Now;
        RequestClient = httpClient;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="clientID">The Client ID generated on Web Interface.</param>
    /// <param name="clientSecret">The Client Secret generated on Web Interface.</param>
    /// <param name="expires">The token expires time (in minutes).</param>
    public Client(string clientID, string clientSecret, long expires)
    {
        AuthBaseUrl = Constants.AUTH_BASE_URL;
        BaseUrl = Constants.BASE_URL;
        Timeout = Constants.API_TIMEOUT;
        Interval = Constants.POOLING_INTERVAL;
        Expires = expires;
        ClientID = clientID;
        ClientSecret = clientSecret;
        Token = string.Empty;
        AutoRefresh = true;
        ExpiresAt = DateTime.Now;
        RequestClient = new HttpClient();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="httpClient">The http client.</param>
    /// <param name="clientID">The Client ID generated on Web Interface.</param>
    /// <param name="clientSecret">The Client Secret generated on Web Interface.</param>
    /// <param name="expires">The token expires time (in minutes).</param>
    public Client(HttpClient httpClient, string clientID, string clientSecret, long expires)
    {
        AuthBaseUrl = Constants.AUTH_BASE_URL;
        BaseUrl = Constants.BASE_URL;
        Timeout = Constants.API_TIMEOUT;
        Interval = Constants.POOLING_INTERVAL;
        Expires = expires;
        ClientID = clientID;
        ClientSecret = clientSecret;
        Token = string.Empty;
        AutoRefresh = true;
        ExpiresAt = DateTime.Now;
        RequestClient = httpClient;
    }

    /// <summary>
    /// Update auto refresh configs.
    /// </summary>
    /// <param name="clientID">The Client ID generated on Web Interface.</param>
    /// <param name="clientSecret">The Client Secret generated on Web Interface.</param>
    /// <param name="expires">The token expires time (in minutes).</param>
    public void SetAutoRefresh(string clientID, string clientSecret, long expires)
    {
        ClientID = clientID;
        ClientSecret = clientSecret;
        Expires = expires;
        AutoRefresh = true;
        ExpiresAt = DateTime.Now;
    }

    /// <summary>
    /// Update base url.
    /// </summary>
    /// <param name="baseUrl">The base url.</param>
    public void SetBaseUrl(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    /// <summary>
    /// Update auth base url.
    /// </summary>
    /// <param name="authBaseUrl">The auth base url.</param>
    public void SetAuthBaseUrl(string authBaseUrl)
    {
        AuthBaseUrl = authBaseUrl;
    }

    /// <summary>
    /// Update pooling interval.
    /// </summary>
    /// <param name="interval">The interval in seconds.</param>
    public void SetInterval(int interval)
    {
        Interval = interval;
    }

    /// <summary>
    /// Update pooling timeout.
    /// </summary>
    /// <param name="timeout">The timeout in seconds.</param>
    public void SetTimeout(int timeout)
    {
        Timeout = timeout;
    }

    /// <summary>
    /// Update the http client.
    /// </summary>
    /// <param name="httpClient">The http client.</param>
    public void SetHttpClient(HttpClient httpClient)
    {
        RequestClient = httpClient;
    }

    /// <summary>
    /// Authenticate on UltraOCR.
    /// </summary>
    /// <param name="clientID">The Client ID generated on Web Interface.</param>
    /// <param name="clientSecret">The Client Secret generated on Web Interface.</param>
    /// <param name="expires">The token expires time (in minutes).</param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task"/> The url info.</returns>
    public async Task Authenticate(string clientID, string clientSecret, long expires)
    {
        Dictionary<string, object> data = new()
        {
            { "ClientID", clientID },
            { "ClientSecret", clientSecret },
            { "ExpiresIn", expires },
        };
        string body = JsonSerializer.Serialize(data);

        HttpRequestMessage message = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthBaseUrl}/token"),
            Content = new StringContent(body),
        };
        message.Headers.TryAddWithoutValidation(Constants.HEADER_ACCEPT, Constants.APPLICATION_JSON);
        message.Headers.TryAddWithoutValidation(Constants.HEADER_CONTENT_TYPE, Constants.APPLICATION_JSON);

        HttpResponseMessage res = await RequestClient.SendAsync(message);
        res.EnsureSuccessStatusCode();

        string result = res.Content.ReadAsStringAsync().Result;
        TokenResponse response = JsonSerializer.Deserialize<TokenResponse>(result)
            ?? throw new InvalidResponseException();

        Token = response.Token;
    }

    /// <summary>
    /// Generate signed url to send the document.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="resource">The way to process, whether job or batch.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{SignedUrlResponse}"/> When authentication is done info.</returns>
    public async Task<SignedUrlResponse> GenerateSignedUrl(
        string service,
        Resource resource,
        object metadata,
        Dictionary<string, string> parameters)
    {
        string url = $"{BaseUrl}/ocr/{resource.ToString().ToLower()}/{service}";

        HttpResponseMessage response = await Post(url, metadata, parameters);
        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<SignedUrlResponse>(result) ?? throw new InvalidResponseException();
    }

    /// <summary>
    /// Upload file given a content.
    /// </summary>
    /// <param name="url">The url to upload the file.</param>
    /// <param name="body">The file content.</param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <returns>A <see cref="Task"/> When upload is done.</returns>
    public async Task UploadFile(string url, byte[] body)
    {
        HttpRequestMessage message = new()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(url),
            Content = new ByteArrayContent(body),
        };

        HttpResponseMessage response = await RequestClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Upload file given a file path.
    /// </summary>
    /// <param name="url">The url to upload the file.</param>
    /// <param name="filePath">The file path.</param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <returns>A <see cref="Task"/> When upload is done.</returns>
    public async Task UploadFileWithPath(string url, string filePath)
    {
        byte[] file = await File.ReadAllBytesAsync(filePath);
        await UploadFile(url, file);
    }

    /// <summary>
    /// Send job in a single step on UltraOCR.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="file">The file on base64 format.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{CreatedResponse}"/> The job info with id.</returns>
    public async Task<CreatedResponse> SendJobSingleStep(
        string service,
        string file,
        Dictionary<string, object> metadata,
        Dictionary<string, string> parameters)
    {
        string url = $"{BaseUrl}/ocr/job/send/{service}";
        Dictionary<string, object> body = new()
        {
            { "data", file },
            { "metadata", metadata },
        };

        HttpResponseMessage response = await Post(url, body, parameters);
        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<CreatedResponse>(result) ?? throw new InvalidResponseException();
    }

    /// <summary>
    /// Send job in a single step on UltraOCR.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="file">The file on base64 format.</param>
    /// <param name="facematchFile">The facematch file on base64 format.</param>
    /// <param name="extraFile">The extra file on base64 format.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{CreatedResponse}"/> The job info with id.</returns>
    public async Task<CreatedResponse> SendJobSingleStep(
        string service,
        string file,
        string facematchFile,
        string extraFile,
        Dictionary<string, object> metadata,
        Dictionary<string, string> parameters)
    {
        string url = $"{BaseUrl}/ocr/job/send/{service}";
        Dictionary<string, object> body = new()
        {
            { "data", file },
            { "metadata", metadata },
        };

        if (parameters.Contains(new(Constants.KEY_FACEMATCH, Constants.FLAG_TRUE)))
        {
            body.Add(Constants.KEY_FACEMATCH, facematchFile);
        }

        if (parameters.Contains(new(Constants.KEY_EXTRA, Constants.FLAG_TRUE)))
        {
            body.Add(Constants.KEY_EXTRA, extraFile);
        }

        HttpResponseMessage response = await Post(url, body, parameters);
        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<CreatedResponse>(result) ?? throw new InvalidResponseException();
    }

    /// <summary>
    /// Send job.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="filePath">The file path of the document.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{CreatedResponse}"/> The job info with id.</returns>
    public async Task<CreatedResponse> SendJob(
        string service,
        string filePath,
        Dictionary<string, object> metadata,
        Dictionary<string, string> parameters)
    {
        SignedUrlResponse response = await GenerateSignedUrl(service, Resource.Job, metadata, parameters);
        await UploadFileWithPath(response.Urls[Constants.KEY_DOCUMENT], filePath);

        return new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
    }

    /// <summary>
    /// Send job.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="filePath">The file path of the document.</param>
    /// <param name="facematchFilePath">The facematch file path of the document.</param>
    /// <param name="extraFilePath">The extra file path of the document.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{CreatedResponse}"/> The job info with id.</returns>
    public async Task<CreatedResponse> SendJob(
        string service,
        string filePath,
        string facematchFilePath,
        string extraFilePath,
        Dictionary<string, object> metadata,
        Dictionary<string, string> parameters)
    {
        SignedUrlResponse response = await GenerateSignedUrl(service, Resource.Job, metadata, parameters);
        await UploadFileWithPath(response.Urls[Constants.KEY_DOCUMENT], filePath);

        if (parameters.Contains(new(Constants.KEY_FACEMATCH, Constants.FLAG_TRUE)))
        {
            await UploadFileWithPath(response.Urls[Constants.KEY_SELFIE], facematchFilePath);
        }

        if (parameters.Contains(new(Constants.KEY_EXTRA, Constants.FLAG_TRUE)))
        {
            await UploadFileWithPath(response.Urls[Constants.KEY_EXTRA_URL], extraFilePath);
        }

        return new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
    }

    /// <summary>
    /// Send job on base64 format.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="file">The file on base64 format.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{CreatedResponse}"/> The job info with id.</returns>
    public async Task<CreatedResponse> SendJobBase64(
        string service,
        byte[] file,
        Dictionary<string, object> metadata,
        Dictionary<string, string> parameters)
    {
        parameters.Add(Constants.BASE64_ATTRIBUTE, Constants.FLAG_TRUE);

        SignedUrlResponse response = await GenerateSignedUrl(service, Resource.Job, metadata, parameters);
        await UploadFile(response.Urls[Constants.KEY_DOCUMENT], file);

        return new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
    }

    /// <summary>
    /// Send job on base64 format.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="file">The file on base64 format.</param>
    /// <param name="facematchFile">The facematch file on base64 format.</param>
    /// <param name="extraFile">The extra file on base64 format.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{CreatedResponse}"/> The job info with id.</returns>
    public async Task<CreatedResponse> SendJobBase64(
        string service,
        byte[] file,
        byte[] facematchFile,
        byte[] extraFile,
        Dictionary<string, object> metadata,
        Dictionary<string, string> parameters)
    {
        parameters.Add(Constants.BASE64_ATTRIBUTE, Constants.FLAG_TRUE);

        SignedUrlResponse response = await GenerateSignedUrl(service, Resource.Job, metadata, parameters);
        await UploadFile(response.Urls[Constants.KEY_DOCUMENT], file);

        if (parameters.Contains(new(Constants.KEY_FACEMATCH, Constants.FLAG_TRUE)))
        {
            await UploadFile(response.Urls[Constants.KEY_SELFIE], facematchFile);
        }

        if (parameters.Contains(new(Constants.KEY_EXTRA, Constants.FLAG_TRUE)))
        {
            await UploadFile(response.Urls[Constants.KEY_EXTRA_URL], extraFile);
        }

        return new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
    }

    /// <summary>
    /// Send batch.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="filePath">The file path of the document.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{CreatedResponse}"/> The batch info with id.</returns>
    public async Task<CreatedResponse> SendBatch(
        string service,
        string filePath,
        Dictionary<string, object>[] metadata,
        Dictionary<string, string> parameters)
    {
        SignedUrlResponse response = await GenerateSignedUrl(service, Resource.Batch, metadata, parameters);
        await UploadFileWithPath(response.Urls[Constants.KEY_DOCUMENT], filePath);

        return new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
    }

    /// <summary>
    /// Send batch on base64 format.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="file">The file on base64 format.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{CreatedResponse}"/> The batch info with id.</returns>
    public async Task<CreatedResponse> SendBatchBase64(
        string service,
        byte[] file,
        Dictionary<string, object>[] metadata,
        Dictionary<string, string> parameters)
    {
        parameters.Add(Constants.BASE64_ATTRIBUTE, Constants.FLAG_TRUE);

        SignedUrlResponse response = await GenerateSignedUrl(service, Resource.Batch, metadata, parameters);
        await UploadFile(response.Urls[Constants.KEY_DOCUMENT], file);

        return new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
    }

    /// <summary>
    /// Get job result.
    /// </summary>
    /// <param name="batchKsuid">the id of the batch, given on batch creation(repeat the job id if batch wasn't created).</param>
    /// <param name="jobKsuid">the id of the job, given on job creation or on batch status.</param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{JobResultResponse}"/> The job response.</returns>
    public async Task<JobResultResponse> GetJobResult(string batchKsuid, string jobKsuid)
    {
        string url = $"{BaseUrl}/ocr/job/result/{batchKsuid}/{jobKsuid}";

        HttpResponseMessage response = await Get(url, []);
        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<JobResultResponse>(result) ?? throw new InvalidResponseException();
    }

    /// <summary>
    /// Get document batch status.
    /// </summary>
    /// <param name="batchKsuid">The id of the batch, given on batch creation.</param>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <returns>A <see cref="Task{BatchStatusResponse}"/> The batch with status.</returns>
    public async Task<BatchStatusResponse> GetBatchStatus(string batchKsuid)
    {
        string url = $"{BaseUrl}/ocr/batch/status/{batchKsuid}";

        HttpResponseMessage response = await Get(url, []);
        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<BatchStatusResponse>(result) ?? throw new InvalidResponseException();
    }

    /// <summary>
    /// Gets the jobs in a time interval.
    /// </summary>
    /// <param name="start">The start date (in the format YYYY-MM-DD).</param>
    /// <param name="end">The end date (in the format YYYY-MM-DD).</param>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{List{JobResultResponse}}"/> The list of jobs.</returns>
    public async Task<List<JobResultResponse>> GetJobs(string start, string end)
    {
        string url = $"{BaseUrl}/ocr/job/results";
        Dictionary<string, string> parameters = new()
        {
            { "startDate", start },
            { "endDate", end },
        };

        List<JobResultResponse> jobs = [];
        bool hasNextPage = true;

        while (hasNextPage)
        {
            HttpResponseMessage res = await Get(url, parameters);
            res.EnsureSuccessStatusCode();

            string result = res.Content.ReadAsStringAsync().Result;
            GetJobsResponse response = JsonSerializer.Deserialize<GetJobsResponse>(result)
                ?? throw new InvalidResponseException();

            string nextPageToken = response.NextPageToken ?? string.Empty;
            parameters.Add("nextPageToken", nextPageToken);

            if (nextPageToken == string.Empty)
            {
                hasNextPage = false;
            }
        }

        return jobs;
    }

    /// <summary>
    /// Wait the job to be processed.
    /// </summary>
    /// <param name="batchKsuid">The id of the batch, given on batch creation(repeat the job id if batch wasn't created).</param>
    /// <param name="jobKsuid">The id of the job, given on job creation or on batch status.</param>
    /// <exception cref="JobTimeoutException">Thrown if timeout is reached.</exception>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{JobResultResponse}"/> The job with result.</returns>
    public async Task<JobResultResponse> WaitForJobDone(string batchKsuid, string jobKsuid)
    {
        DateTime end = DateTime.Now.AddSeconds(Timeout);
        while (true)
        {
            JobResultResponse response = await GetJobResult(batchKsuid, jobKsuid);
            string status = response.Status;

            if (status == Constants.STATUS_DONE || status == Constants.STATUS_ERROR)
            {
                return response;
            }

            if (DateTime.Now > end)
            {
                throw new JobTimeoutException(Timeout);
            }

            Thread.Sleep(Interval * 1000);
        }
    }

    /// <summary>
    /// Wait the batch to be processed.
    /// </summary>
    /// <param name="batchKsuid">The id of the batch, given on batch creation.</param>
    /// <param name="waitJobs">Indicate if must wait the jobs to be processed.</param>
    /// <exception cref="JobTimeoutException">Thrown if timeout is reached.</exception>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{BatchStatusResponse}"/> The batch with status.</returns>
    public async Task<BatchStatusResponse> WaitForBatchDone(string batchKsuid, bool waitJobs)
    {
        DateTime end = DateTime.Now.AddSeconds(Timeout);
        BatchStatusResponse response;

        while (true)
        {
            response = await GetBatchStatus(batchKsuid);
            string status = response.Status;

            if (status == Constants.STATUS_DONE || status == Constants.STATUS_ERROR)
            {
                break;
            }

            if (DateTime.Now > end)
            {
                throw new JobTimeoutException(Timeout);
            }

            Thread.Sleep(Interval * 1000);
        }

        if (waitJobs)
        {
            foreach (BatchStatusJobs job in response.Jobs ?? [])
            {
                await WaitForJobDone(batchKsuid, job.JobKsuid);
            }
        }

        return response;
    }

    /// <summary>
    /// Create and wait for job done.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="filePath">The file path of the document.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="JobTimeoutException">Thrown if timeout is reached.</exception>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{JobResultResponse}"/> The job with result.</returns>
    public async Task<JobResultResponse> CreateAndWaitJob(
        string service,
        string filePath,
        Dictionary<string, object> metadata,
        Dictionary<string, string> parameters)
    {
        CreatedResponse response = await SendJob(service, filePath, metadata, parameters);
        return await WaitForJobDone(response.Id, response.Id);
    }

    /// <summary>
    /// Create and wait for job done.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="filePath">The file path of the document.</param>
    /// <param name="facematchFilePath">The facematch file path of the document.</param>
    /// <param name="extraFilePath">The extra file path of the document.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <exception cref="JobTimeoutException">Thrown if timeout is reached.</exception>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{JobResultResponse}"/> The job with result.</returns>
    public async Task<JobResultResponse> CreateAndWaitJob(
        string service,
        string filePath,
        string facematchFilePath,
        string extraFilePath,
        Dictionary<string, object> metadata,
        Dictionary<string, string> parameters)
    {
        CreatedResponse response = await SendJob(
            service,
            filePath,
            facematchFilePath,
            extraFilePath,
            metadata,
            parameters);
        return await WaitForJobDone(response.Id, response.Id);
    }

    /// <summary>
    /// Create and wait for batch done.
    /// </summary>
    /// <param name="service">The the type of document to be sent.</param>
    /// <param name="filePath">The file path of the document.</param>
    /// <param name="metadata">The metadata based on UltraOCR Docs format, optional in most cases.</param>
    /// <param name="parameters">The query parameters based on UltraOCR Docs, optional in most cases. </param>
    /// <param name="waitJobs">Indicate if must wait the jobs to be processed.</param>
    /// <exception cref="JobTimeoutException">Thrown if timeout is reached.</exception>
    /// <exception cref="HttpRequestException">Thrown if wrong status code.</exception>
    /// <exception cref="InvalidResponseException">Thrown if response is invalid.</exception>
    /// <returns>A <see cref="Task{BatchStatusResponse}"/> The batch with status.</returns>
    public async Task<BatchStatusResponse> CreateAndWaitBatch(
        string service,
        string filePath,
        Dictionary<string, object>[] metadata,
        Dictionary<string, string> parameters,
        bool waitJobs)
    {
        CreatedResponse response = await SendBatch(service, filePath, metadata, parameters);
        return await WaitForBatchDone(response.Id, waitJobs);
    }

    private static string GetFullUrl(string url, Dictionary<string, string> parameters)
    {
        List<string> parsed = [];
        foreach (var item in parameters)
        {
            parsed.Add($"{item.Key}={item.Value}");
        }

        return $"{url}?{string.Join("&", parsed)}";
    }

    private async Task AutoAuthenticate()
    {
        if (AutoRefresh && (ExpiresAt >= DateTime.Now))
        {
            await Authenticate(ClientID, ClientSecret, Expires);
        }
    }

    private async Task<HttpResponseMessage> Post(string url, object data, Dictionary<string, string> parameters)
    {
        await AutoAuthenticate();
        string body = JsonSerializer.Serialize(data);

        HttpRequestMessage message = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(GetFullUrl(url, parameters)),
            Content = new StringContent(body),
        };
        message.Headers.TryAddWithoutValidation(Constants.HEADER_ACCEPT, Constants.APPLICATION_JSON);
        message.Headers.TryAddWithoutValidation(Constants.HEADER_CONTENT_TYPE, Constants.APPLICATION_JSON);
        message.Headers.TryAddWithoutValidation(Constants.HEADER_AUTHORIZATION, Constants.BEARER_PREFIX + Token);

        return await RequestClient.SendAsync(message);
    }

    private async Task<HttpResponseMessage> Get(string url, Dictionary<string, string> parameters)
    {
        await AutoAuthenticate();

        HttpRequestMessage message = new()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(GetFullUrl(url, parameters)),
        };
        message.Headers.TryAddWithoutValidation(Constants.HEADER_ACCEPT, Constants.APPLICATION_JSON);
        message.Headers.TryAddWithoutValidation(Constants.HEADER_CONTENT_TYPE, Constants.APPLICATION_JSON);
        message.Headers.TryAddWithoutValidation(Constants.HEADER_AUTHORIZATION, Constants.BEARER_PREFIX + Token);

        return await RequestClient.SendAsync(message);
    }
}
