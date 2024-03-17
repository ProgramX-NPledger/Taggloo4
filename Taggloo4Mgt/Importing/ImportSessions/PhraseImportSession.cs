using System.Net.Http.Json;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;
using Taggloo4Mgt.Importing.Importers;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.ImportSessions;

public class PhraseImportSession : IImportSession
{
    private readonly IEnumerable<Phrase> _phrases;

    public PhraseImportSession(IEnumerable<Phrase> phrases)
    {
        _phrases = phrases;
    }
    
    public int GetToBeImportedCount()
    {
        return _phrases.Count();
    }


    public event EventHandler<ImportEventArgs>? LogMessage;
    public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
    public event EventHandler<ImportedEventArgs>? Imported;
    
    public async Task Import(HttpClient httpClient, string languageCode, int dictionaryId, Dictionary<string, Dictionary<int, string>> originalIdsToImportIdsMap)
    {
        Phrase[] phrasesInLanguage =
        		    _phrases.Where(q => q.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();
        
        LogMessage?.Invoke(this,new ImportEventArgs()
        {
        	LogMessage = $"Importing {phrasesInLanguage.Count()} Phrases for Language {languageCode}",
        	Indentation = 4
        });
        
        
        // for each Phrase in source
        foreach (Phrase phraseInLanguage in phrasesInLanguage)
        {
        	DateTime startTimeStamp = DateTime.Now;
        	
        	LogMessage?.Invoke(this,new ImportEventArgs()
        	{
        		LogMessage = $"{phraseInLanguage.ThePhrase}",
        		Indentation = 5
        	});

        	try
        	{
		        // verify that word doesn't already exist
		        bool phraseAlreadyExists = await IsPhraseExtant(httpClient, phraseInLanguage, dictionaryId);
		        if (!phraseAlreadyExists)
		        {
			        CreatePhraseResult createPhraseResult = await PostPhraseToTarget(httpClient, phraseInLanguage, dictionaryId);
			        Imported?.Invoke(this,new ImportedEventArgs()
			        {
				        LanguageCode = languageCode,
				        CurrentItem = phraseInLanguage.ThePhrase,
				        IsSuccess = true,
				        SourceId = phraseInLanguage.ID
			        });
		        }
		        else
		        {
			        throw new ImportException(
				        $"Phrase '{phraseInLanguage}' already exists in Dictionary ID {dictionaryId}");
		        }
        	}
        	catch (Exception ex)
        	{
        		await Console.Error.WriteLineAsync();
        		await Console.Error.WriteLineAsync($"Failed to import Word '{phraseInLanguage.ThePhrase}'");
        		Exception? exPtr = ex;
        		do
        		{
        			await Console.Error.WriteLineAsync(exPtr.Message);
        			LogMessage?.Invoke(this, new ImportEventArgs()
        			{
        				LogMessage = $"ERROR: {exPtr.GetType().Name}: {exPtr.Message}",
        				Indentation = 6
        			});
        			exPtr = exPtr.InnerException;
        		} while (exPtr!=null);
        		Imported?.Invoke(this,new ImportedEventArgs()
        		{
        			LanguageCode = languageCode,
        			CurrentItem = phraseInLanguage.ThePhrase,
        			IsSuccess = false
        		});
        	}
        		
        	TimeSpan delta = DateTime.Now - startTimeStamp;
        	
        	UpdateMetrics?.Invoke(this,new ImportMetricsEventArgs()
        	{
        		MillisecondsBetweenImports = delta.TotalMilliseconds,
        	});
        	
        }
		        
    }

    private async Task<bool> IsPhraseExtant(HttpClient httpClient, Phrase phrase, int dictionaryId)
    {
	    string url = $"/api/v4/phrases?phrase={phrase}&dictionaryId={dictionaryId}";
	    HttpResponseMessage response = await httpClient.GetAsync(url);
	    response.EnsureSuccessStatusCode();
	    
	    GetPhrasesResult? getPhrasesResult = await response.Content.ReadFromJsonAsync<GetPhrasesResult>();
	    if (getPhrasesResult == null) throw new NullReferenceException("getPhrasesResult");

	    IEnumerable<GetPhraseResultItem> matchingDictionary=getPhrasesResult.Results.Where(q =>
		    q.DictionaryId==dictionaryId).ToArray();

	    return matchingDictionary.Any();
    }


    private async Task<CreatePhraseResult> PostPhraseToTarget(HttpClient httpClient, Phrase phrase,
	    int dictionaryId)
    {
	    string url = "/api/v4/phrases";
	    CreatePhrase createPhrase = new CreatePhrase()
	    {
		    Phrase = phrase.ThePhrase,
		    DictionaryId = dictionaryId,
		    CreatedAt = phrase.CreatedTimeStamp,
		    CreatedByUserName = phrase.CreatedByUserName,
		    ExternalId = $"Taggloo2-Phrase-{phrase.ID}"
	    };
		
	    HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createPhrase);
	    if (!response.IsSuccessStatusCode)
	    {
		    throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
	    }

	    CreatePhraseResult? createPhraseResult=
		    await response.Content.ReadFromJsonAsync<CreatePhraseResult>();
	    return createPhraseResult!;
    }

    
}