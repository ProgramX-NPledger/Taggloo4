using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Taggloo4.Dto;

namespace Taggloo4Mgt;

public class Initialiser : ApiClientBase
{
    private readonly InitOptions _initOptions;
    private readonly string? _logFileName;

    public Initialiser(InitOptions initOptions)
    {
        _initOptions = initOptions;
        _logFileName = CreateRandomLogFileName();
    }

    private string CreateRandomLogFileName()
    {
        return $"log{DateTime.Now:yyyyMMddHHmm}.txt";
    }

    public async Task<int> Process()
    {
        if (_initOptions.Log) Console.WriteLine($"Logging to {_logFileName}");

        // attempt to start an initialisation
        Log($"Connect to API at {_initOptions.Url}");
        using (HttpClient httpClient = CreateHttpClient(_initOptions.Url))
        {
            Log("\tOk");

            PrepareForInitialisation(httpClient);

            // periodically get status


        }

        return 0;
    }

    private async Task<BeginInitialisationResult> PrepareForInitialisation(HttpClient httpClient)
    {
        string url = "/api/v4/init";
        BeginInitialisation beginInitialisation = new BeginInitialisation()
        {
        };
	
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, beginInitialisation);
        switch (response.StatusCode)
        {
            case HttpStatusCode.Locked:
                // initialisation is not permitted
                throw new InvalidOperationException($"API refuses to initialise.");
                break;
            case HttpStatusCode.Accepted:
                // initialisation has been accepted, location can be used to check status
                break;
            default:
                throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
        }

        BeginInitialisationResult? beginInitialisationResult=
            await response.Content.ReadFromJsonAsync<BeginInitialisationResult>();
        return beginInitialisationResult!;
    }

    
    private void Log(string s)
    {
        if (_initOptions.Log)
        {
            File.AppendAllLines(_logFileName!,new string[]
            {
                s
            });
        }
    }
    
}