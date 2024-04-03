using System.Net;
using System.Net.Http.Json;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;
using Taggloo4Mgt.Importing.Importers;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.ImportSessions;

public class WordImportSession : IImportSession
{
    private readonly IEnumerable<Word> _words;

    // ReSharper disable once ConvertToPrimaryConstructor
    public WordImportSession(IEnumerable<Word> words)
    {
        _words = words;
    }

    public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
    public event EventHandler<ImportedEventArgs>? Imported;

    public async Task ImportAcrossDictionariesAsync(HttpClient httpClient, string languageCode1, int dictionary1Id, string languageCode2,
	    int dictionary2Id)
    {
	    // words are simple, we can work with languages individually
	    await ImportWithinDictionaryAsync(httpClient, languageCode1, dictionary1Id);
	    await ImportWithinDictionaryAsync(httpClient, languageCode2, dictionary2Id);
    }

    private async Task ImportWithinDictionaryAsync(HttpClient httpClient, string languageCode, int dictionaryId)
    {
	    Word[] wordsInLanguage =
		    _words.Where(q => q.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();
	    
		LogMessage?.Invoke(this,new ImportEventArgs()
		{
			LogMessage = $"Importing {wordsInLanguage.Count()} Words for Language {languageCode}",
			Indentation = 4
		});
		
		
		// for each Word in source
		foreach (Word wordInLanguage in wordsInLanguage)
		{
			DateTime startTimeStamp = DateTime.Now;
			
			LogMessage?.Invoke(this,new ImportEventArgs()
			{
				LogMessage = $"{wordInLanguage.TheWord}",
				Indentation = 5
			});

			
			try
			{
				// verify that word doesn't already exist
				bool wordAlreadyExists = await IsWordExtant(httpClient, wordInLanguage, dictionaryId);
				if (!wordAlreadyExists)
				{
					CreateWordResult createWordResult = await PostWordToTarget(httpClient, wordInLanguage, dictionaryId);
					Imported?.Invoke(this,new ImportedEventArgs()
					{
						LanguageCode = languageCode,
						CurrentItem = wordInLanguage.TheWord,
						ExternalId = createWordResult.ExternalId,
						IsSuccess = true,
						SourceId = wordInLanguage.ID
					});
				}
				else
				{
					throw new ImportException(
						$"Word '{wordInLanguage.TheWord}' already exists in Dictionary ID {dictionaryId}");
				}
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync();
				await Console.Error.WriteLineAsync($"Failed to import Word '{wordInLanguage.TheWord}'");
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
					CurrentItem = wordInLanguage.TheWord,
					IsSuccess = false,
					SourceId = wordInLanguage.ID
				});
			}
				
			TimeSpan delta = DateTime.Now - startTimeStamp;
			
			UpdateMetrics?.Invoke(this,new ImportMetricsEventArgs()
			{
				MillisecondsBetweenImports = delta.TotalMilliseconds,
			});
			
		}
    }

    public event EventHandler<ImportEventArgs>? LogMessage;
    
    public int GetToBeImportedCount()
    {
        return _words.Count();
    }

    

    private async Task<bool> IsWordExtant(HttpClient httpClient, Word word, int dictionaryId)
    {
	    string url = $"/api/v4/words?word={word}&dictionaryId={dictionaryId}";
	    HttpResponseMessage response = await httpClient.GetAsync(url);
	    response.EnsureSuccessStatusCode();
	    
	    GetWordsResult? getWordsResult = await response.Content.ReadFromJsonAsync<GetWordsResult>();
	    if (getWordsResult == null) throw new NullReferenceException("getWordsResult");

	    IEnumerable<GetWordResultItem> matchingDictionary=getWordsResult.Results.Where(q =>
		    q.DictionaryId==dictionaryId).ToArray();

	    return matchingDictionary.Any();
	    
    }


    private async Task<CreateWordResult> PostWordToTarget(HttpClient httpClient, Word word,
		int dictionaryId)
	{
		string url = "/api/v4/words";
		CreateWord createWord = new CreateWord()
		{
			Word = word.TheWord,
			CreatedAt = word.CreatedTimeStamp,
			CreatedByUserName = word.CreatedByUserName,
			DictionaryId = dictionaryId,
			ExternalId = $"Taggloo2-Word-{word.ID}"
		};
		
		HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createWord);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}
		
		CreateWordResult? createWordResult =
			await response.Content.ReadFromJsonAsync<CreateWordResult>();
		if (createWordResult == null) throw new NullReferenceException("createWordResult");
		return createWordResult; 
	}
	
	
}