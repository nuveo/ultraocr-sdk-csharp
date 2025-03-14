namespace ultraocr;

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
    private HttpClient HttpClient { get; set; }

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
        HttpClient = new HttpClient();
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
        HttpClient = httpClient;
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
        HttpClient = new HttpClient();
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
        HttpClient = httpClient;
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
        HttpClient = httpClient;
    }
}
