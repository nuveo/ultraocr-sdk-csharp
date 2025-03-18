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
    private long Interval { get; set; }
    private bool AutoRefresh { get; set; }
    private HttpClient RequestClient { get; set; }

    public Client() {
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

    public Client(HttpClient httpClient) {
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

    public Client(string clientID, string clientSecret, long expires) {
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

    public Client(HttpClient httpClient, string clientID, string clientSecret, long expires) {
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

    public void SetBaseUrl(string baseUrl) {
        BaseUrl = baseUrl;
    }

    public void SetAuthBaseUrl(string authBaseUrl) {
        AuthBaseUrl = authBaseUrl;
    }

    public void SetInterval(long interval) {
        Interval = interval;
    }

    public void SetHttpClient(HttpClient httpClient) {
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

    private static string GetFullUrl(string url, Dictionary<string, string> parameters) {
        var parsed = new List<string>();

        foreach(var item in parameters)
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

    public async Task<BatchStatusResponse?> GetBatchStatus(string batchKsuid) {
        string url = $"{BaseUrl}/ocr/batch/status/{batchKsuid}";
        HttpResponseMessage response = await Get(url, []);

        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        var res = JsonSerializer.Deserialize<BatchStatusResponse>(result);

        return res;
    }

    public async Task<JobResultResponse?> GetJobResult(string batchKsuid, string jobKsuid) {
        string url = $"{BaseUrl}/ocr/job/result/{batchKsuid}/{jobKsuid}";
        HttpResponseMessage response = await Get(url, []);

        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        var res = JsonSerializer.Deserialize<JobResultResponse>(result);

        return res;
    }

    public async Task<JobResultResponse?> GetJobResult(string jobKsuid) {
        string url = $"{BaseUrl}/ocr/job/result/{jobKsuid}/{jobKsuid}";
        HttpResponseMessage response = await Get(url, []);

        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        var res = JsonSerializer.Deserialize<JobResultResponse>(result);

        return res;
    }

    public async Task<SignedUrlResponse?> GenerateSignedUrl(string service, string resource, object metadata, Dictionary<string, string> parameters) {
        string url = $"{BaseUrl}/ocr/{resource}/{service}";
        HttpResponseMessage response = await Post(url, metadata, parameters);

        response.EnsureSuccessStatusCode();

        string result = response.Content.ReadAsStringAsync().Result;
        var res = JsonSerializer.Deserialize<SignedUrlResponse>(result);

        return res;
    }
}
