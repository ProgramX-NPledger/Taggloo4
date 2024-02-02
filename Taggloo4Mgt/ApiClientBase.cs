namespace Taggloo4Mgt;

public class ApiClientBase
{
    protected HttpClient CreateHttpClient(string url)
    {
        HttpClientHandler httpClientHandler = new HttpClientHandler();
#if DEBUG
        httpClientHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif
		
        HttpClient httpClient = new HttpClient(httpClientHandler);
        // Server rejects when added this header
        // httpClient.DefaultRequestHeaders.Accept.Clear();
        // httpClient.DefaultRequestHeaders.Accept.Add(
        // 	new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Taggloo4Mgt utility");
        httpClient.BaseAddress = new Uri(url);
		
        return httpClient;
    }

}