using System.Net.Http.Json;
using Taggloo4.Dto;
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
    public event EventHandler<ImportEventArgs>? LogMessage;
    
    public int GetToBeImportedCount()
    {
        return _words.Count();
    }

    public async Task Import(HttpClient httpClient, string languageCode, int dictionaryId, Dictionary<string, Dictionary<int, Guid>> originalIdsToImportIdsMap)
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
				CreateWordResult createWordResult = await PostWordToTarget(httpClient, wordInLanguage, dictionaryId);
				Imported?.Invoke(this,new ImportedEventArgs()
				{
					LanguageCode = languageCode,
					CurrentItem = wordInLanguage.TheWord,
					ImportGuid = createWordResult.ImportId,
					IsSuccess = true,
					SourceId = wordInLanguage.ID
				});
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

    
    private async Task<CreateWordResult> PostWordToTarget(HttpClient httpClient, Word word,
		int dictionaryId)
	{
		Thread.Sleep(1000);
		
		string url = "/api/v4/words";
		CreateWord createWord = new CreateWord()
		{
			Word = word.TheWord,
			CreatedAt = word.CreatedTimeStamp,
			CreatedByUserName = word.CreatedByUserName,
			DictionaryId = dictionaryId
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