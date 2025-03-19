using System.Text.Json;
using System.Threading.Tasks;
using ultraocr;
using ultraocr.Responses;

namespace Ultraocr;

class Client
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

    public Client()
    {
        AuthBaseUrl = Constants.AUTH_BASE_URL;
        BaseUrl = Constants.BASE_URL;
        Timeout = Constants.API_TIMEOUT;
        Interval = Constants.POOLING_INTERVAL;
        Expires = Constants.DEFAULT_EXPIRATION_TIME;
        AutoRefresh = false;
        ClientID = "";
        ClientSecret = "";
        Token = "";
        ExpiresAt = DateTime.Now;
        RequestClient = new HttpClient();
    }

    public Client(HttpClient httpClient)
    {
        AuthBaseUrl = Constants.AUTH_BASE_URL;
        BaseUrl = Constants.BASE_URL;
        Timeout = Constants.API_TIMEOUT;
        Interval = Constants.POOLING_INTERVAL;
        Expires = Constants.DEFAULT_EXPIRATION_TIME;
        ClientID = "";
        ClientSecret = "";
        Token = "";
        AutoRefresh = false;
        ExpiresAt = DateTime.Now;
        RequestClient = httpClient;
    }

    public Client(string clientID, string clientSecret, long expires)
    {
        AuthBaseUrl = Constants.AUTH_BASE_URL;
        BaseUrl = Constants.BASE_URL;
        Timeout = Constants.API_TIMEOUT;
        Interval = Constants.POOLING_INTERVAL;
        Expires = expires;
        ClientID = clientID;
        ClientSecret = clientSecret;
        Token = "";
        AutoRefresh = true;
        ExpiresAt = DateTime.Now;
        RequestClient = new HttpClient();
    }

    public Client(HttpClient httpClient, string clientID, string clientSecret, long expires)
    {
        AuthBaseUrl = Constants.AUTH_BASE_URL;
        BaseUrl = Constants.BASE_URL;
        Timeout = Constants.API_TIMEOUT;
        Interval = Constants.POOLING_INTERVAL;
        Expires = expires;
        ClientID = clientID;
        ClientSecret = clientSecret;
        Token = "";
        AutoRefresh = true;
        ExpiresAt = DateTime.Now;
        RequestClient = httpClient;
    }


    public void SetAutoRefresh(string clientID, string clientSecret, long expires)
    {
        ClientID = clientID;
        ClientSecret = clientSecret;
        Expires = expires;
        AutoRefresh = true;
        ExpiresAt = DateTime.Now;
    }

    public void SetBaseUrl(string baseUrl)
    {
        BaseUrl = baseUrl;
    }

    public void SetAuthBaseUrl(string authBaseUrl)
    {
        AuthBaseUrl = authBaseUrl;
    }

    public void SetInterval(int interval)
    {
        Interval = interval;
    }

    public void SetHttpClient(HttpClient httpClient)
    {
        RequestClient = httpClient;
    }

    public async Task Authenticate(string clientID, string clientSecret, long expires)
    {
        Dictionary<string, object> data = new()
        {
            { "ClientID", clientID },
            { "ClientSecret", clientSecret },
            { "ExpiresIn", expires }
        };
        string body = JsonSerializer.Serialize(data);

        HttpRequestMessage msg = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthBaseUrl}/token"),
            Content = new StringContent(body),
        };
        msg.Headers.TryAddWithoutValidation(Constants.HEADER_ACCEPT, Constants.APPLICATION_JSON);
        msg.Headers.TryAddWithoutValidation(Constants.HEADER_CONTENT_TYPE, Constants.APPLICATION_JSON);

        HttpResponseMessage response = await RequestClient.SendAsync(msg);
        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        var res = JsonSerializer.Deserialize<TokenResponse>(result);
        if (res is not null)
        {
            Token = res.Token;
        }
    }

    private void AutoAuthenticate()
    {
        if (AutoRefresh && (ExpiresAt >= DateTime.Now))
        {
            _ = Authenticate(ClientID, ClientSecret, Expires);
        }
    }

    private static string GetFullUrl(string url, Dictionary<string, string> parameters)
    {
        var parsed = new List<string>();

        foreach (var item in parameters)
        {
            parsed.Add($"{item.Key}={item.Value}");
        }

        return $"{url}?{string.Join("&", parsed)}";
    }

    private async Task<HttpResponseMessage> Post(string url, object data, Dictionary<string, string> parameters)
    {
        AutoAuthenticate();

        string body = JsonSerializer.Serialize(data);

        HttpRequestMessage msg = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(GetFullUrl(url, parameters)),
            Content = new StringContent(body),
        };
        msg.Headers.TryAddWithoutValidation(Constants.HEADER_ACCEPT, Constants.APPLICATION_JSON);
        msg.Headers.TryAddWithoutValidation(Constants.HEADER_CONTENT_TYPE, Constants.APPLICATION_JSON);
        msg.Headers.TryAddWithoutValidation(Constants.HEADER_AUTHORIZATION, Constants.BEARER_PREFIX + Token);

        return await RequestClient.SendAsync(msg);
    }

    private async Task<HttpResponseMessage> Get(string url, Dictionary<string, string> parameters)
    {
        AutoAuthenticate();

        HttpRequestMessage msg = new()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(GetFullUrl(url, parameters)),
        };
        msg.Headers.TryAddWithoutValidation(Constants.HEADER_ACCEPT, Constants.APPLICATION_JSON);
        msg.Headers.TryAddWithoutValidation(Constants.HEADER_CONTENT_TYPE, Constants.APPLICATION_JSON);
        msg.Headers.TryAddWithoutValidation(Constants.HEADER_AUTHORIZATION, Constants.BEARER_PREFIX + Token);

        return await RequestClient.SendAsync(msg);
    }

    public async Task<SignedUrlResponse?> GenerateSignedUrl(string service, string resource, object metadata, Dictionary<string, string> parameters)
    {
        string url = $"{BaseUrl}/ocr/{resource}/{service}";
        HttpResponseMessage response = await Post(url, metadata, parameters);

        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        var res = JsonSerializer.Deserialize<SignedUrlResponse>(result);

        return res;
    }

    public async Task UploadFile(string url, byte[] body)
    {
        HttpRequestMessage msg = new()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(url),
            Content = new ByteArrayContent(body),
        };

        HttpResponseMessage response = await RequestClient.SendAsync(msg);
        response.EnsureSuccessStatusCode();
    }

    public async Task UploadFileWithPath(string url, string filePath)
    {
        byte[] file = await File.ReadAllBytesAsync(filePath);
        await UploadFile(url, file);
    }

    public async Task<CreatedResponse?> SendJobSingleStep(string service, string file, Dictionary<string, object> metadata, Dictionary<string, string> parameters)
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
        var res = JsonSerializer.Deserialize<CreatedResponse>(result);

        return res;
    }

    public async Task<CreatedResponse?> SendJobSingleStep(string service, string file, string facematchFile, string extraFile, Dictionary<string, object> metadata, Dictionary<string, string> parameters)
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
        var res = JsonSerializer.Deserialize<CreatedResponse>(result);

        return res;
    }

    public async Task<CreatedResponse?> SendJob(string service, string filePath, Dictionary<string, object> metadata, Dictionary<string, string> parameters)
    {
        SignedUrlResponse response = await GenerateSignedUrl(service, "job", metadata, parameters);
        await UploadFileWithPath(response.Urls[Constants.KEY_DOCUMENT], filePath);

        CreatedResponse res = new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
        return res;
    }

    public async Task<CreatedResponse?> SendJob(string service, string filePath, string facematchFilePath, string extraFilePath, Dictionary<string, object> metadata, Dictionary<string, string> parameters)
    {
        SignedUrlResponse response = await GenerateSignedUrl(service, "job", metadata, parameters);
        await UploadFileWithPath(response.Urls[Constants.KEY_DOCUMENT], filePath);

        if (parameters.Contains(new(Constants.KEY_FACEMATCH, Constants.FLAG_TRUE)))
        {
            await UploadFileWithPath(response.Urls[Constants.KEY_SELFIE], facematchFilePath);
        }
        if (parameters.Contains(new(Constants.KEY_EXTRA, Constants.FLAG_TRUE)))
        {
            await UploadFileWithPath(response.Urls[Constants.KEY_EXTRA_URL], extraFilePath);
        }

        CreatedResponse res = new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
        return res;
    }

    public async Task<CreatedResponse?> SendJobBase64(string service, byte[] file, Dictionary<string, object> metadata, Dictionary<string, string> parameters)
    {
        parameters.Add(Constants.BASE64_ATTRIBUTE, Constants.FLAG_TRUE);
        SignedUrlResponse response = await GenerateSignedUrl(service, "job", metadata, parameters);
        await UploadFile(response.Urls[Constants.KEY_DOCUMENT], file);

        CreatedResponse res = new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
        return res;
    }

    public async Task<CreatedResponse?> SendJobBase64(string service, byte[] file, byte[] facematchFile, byte[] extraFile, Dictionary<string, object> metadata, Dictionary<string, string> parameters)
    {
        SignedUrlResponse response = await GenerateSignedUrl(service, "job", metadata, parameters);
        await UploadFile(response.Urls[Constants.KEY_DOCUMENT], file);

        if (parameters.Contains(new(Constants.KEY_FACEMATCH, Constants.FLAG_TRUE)))
        {
            await UploadFile(response.Urls[Constants.KEY_SELFIE], facematchFile);
        }
        if (parameters.Contains(new(Constants.KEY_EXTRA, Constants.FLAG_TRUE)))
        {
            await UploadFile(response.Urls[Constants.KEY_EXTRA_URL], extraFile);
        }

        CreatedResponse res = new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
        return res;
    }

    public async Task<CreatedResponse?> SendBatch(string service, string filePath, Dictionary<string, object>[] metadata, Dictionary<string, string> parameters)
    {
        SignedUrlResponse response = await GenerateSignedUrl(service, "batch", metadata, parameters);
        await UploadFileWithPath(response.Urls[Constants.KEY_DOCUMENT], filePath);

        CreatedResponse res = new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
        return res;
    }

    public async Task<CreatedResponse?> SendBatchBase64(string service, byte[] file, Dictionary<string, object>[] metadata, Dictionary<string, string> parameters)
    {
        parameters.Add(Constants.BASE64_ATTRIBUTE, Constants.FLAG_TRUE);
        SignedUrlResponse response = await GenerateSignedUrl(service, "batch", metadata, parameters);
        await UploadFile(response.Urls[Constants.KEY_DOCUMENT], file);

        CreatedResponse res = new()
        {
            StatusUrl = response.StatusUrl,
            Id = response.Id,
        };
        return res;
    }

    public async Task<JobResultResponse?> GetJobResult(string batchKsuid, string jobKsuid)
    {
        string url = $"{BaseUrl}/ocr/job/result/{batchKsuid}/{jobKsuid}";
        HttpResponseMessage response = await Get(url, []);

        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        var res = JsonSerializer.Deserialize<JobResultResponse>(result);

        return res;
    }

    public async Task<BatchStatusResponse?> GetBatchStatus(string batchKsuid)
    {
        string url = $"{BaseUrl}/ocr/batch/status/{batchKsuid}";
        HttpResponseMessage response = await Get(url, []);

        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        var res = JsonSerializer.Deserialize<BatchStatusResponse>(result);

        return res;
    }

    public async Task<List<JobResultResponse>> GetJobs(string start, string end)
    {
        string url = $"{BaseUrl}/ocr/job/results";
        Dictionary<string, string> parameters = new()
        {
            { "startDate", start },
            { "endDate", end },
        };

        var jobs = new List<JobResultResponse>();
        var hasNextPage = true;

        while (hasNextPage)
        {
            HttpResponseMessage response = await Get(url, parameters);

            response.EnsureSuccessStatusCode();

            string result = response.Content.ReadAsStringAsync().Result;
            var res = JsonSerializer.Deserialize<GetJobsResponse>(result);

            var nextPageToken = res.NextPageToken;

            if (nextPageToken == null || nextPageToken == "")
            {
                hasNextPage = false;
                continue;
            }

            parameters.Add("nextPageToken", nextPageToken);
        }

        return jobs;
    }

    public async Task<JobResultResponse> WaitForJobDone(string batchKsuid, string jobKsuid)
    {
        DateTime end = DateTime.Now.AddSeconds(Timeout);
        JobResultResponse response;

        while (true)
        {
            response = await GetJobResult(batchKsuid, jobKsuid);
            string status = response.Status;
            if (status == Constants.STATUS_DONE || status == Constants.STATUS_ERROR)
            {
                return response;
            }

            if (DateTime.Now > end)
            {
                throw new Exception("timeout");
            }

            Thread.Sleep(Interval * 1000);
        }
    }

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
                throw new Exception("timeout");
            }

            Thread.Sleep(Interval * 1000);
        }

        if (waitJobs)
        {
            foreach (var job in response.Jobs)
            {
                WaitForJobDone(batchKsuid, job.JobKsuid);
            }
        }

        return response;
    }
}
