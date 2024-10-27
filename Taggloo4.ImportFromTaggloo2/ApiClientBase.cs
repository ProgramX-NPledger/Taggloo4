namespace Taggloo4Mgt;

public class ApiClientBase
{
    protected HttpClient CreateHttpClient(IHttpClientFactory httpClientFactory, string url)
    {
        
            
//         HttpClientHandler httpClientHandler = new HttpClientHandler();
// #if DEBUG
//         httpClientHandler.ServerCertificateCustomValidationCallback =
//             HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
// #endif
//         httpClientHandler.MaxConnectionsPerServer = 1;

        HttpClient httpClient = httpClientFactory.CreateClient(); // new HttpClient(httpClientHandler);
        // Server rejects when added this header
        // httpClient.DefaultRequestHeaders.Accept.Clear();
        // httpClient.DefaultRequestHeaders.Accept.Add(
        // 	new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Taggloo4.ImportFromTaggloo2 utility");
        httpClient.BaseAddress = new Uri(url);
        httpClient.Timeout = TimeSpan.FromMinutes(10);
        
        return httpClient;
    }

}