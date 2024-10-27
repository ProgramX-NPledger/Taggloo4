using System.Net.Http.Json;
using Taggloo4.Dto;
using Taggloo4Mgt.Importing.Importers;

namespace Taggloo4.ImportFromTaggloo2.Helpers;

public class ImporterHelper
{
    public static async Task<CreateDictionaryResult> CreateDictionaryForLanguage(HttpClient httpClient, string contentType, string languageCode, IImportSession importSession)
    {
        string url = "/api/v4/dictionaries";
        CreateDictionary createDictionary = new CreateDictionary()
        {
            Name = $"{contentType} for Language {languageCode}",
            IetfLanguageTag = languageCode,
            SourceUrl = "https://taggloo.im",
            Description = $"Imported from SQL Server Taggloo2 database by {nameof(importSession)}.",
            ContentTypeKey = importSession.ContentTypeKey
        };
		
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createDictionary);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
        }

        CreateDictionaryResult? createDictionaryResult =
            await response.Content.ReadFromJsonAsync<CreateDictionaryResult>();
        return createDictionaryResult!;
    }
}