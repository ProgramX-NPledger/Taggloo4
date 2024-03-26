using System.Net;
using System.Net.Http.Json;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;
using Taggloo4Mgt.Importing.Importers;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.ImportSessions;

public class WordTranslationImportSession : IImportSession
{
	private readonly IEnumerable<WordTranslation> _wordTranslations;

	public WordTranslationImportSession(IEnumerable<WordTranslation> wordTranslations)
	{
		_wordTranslations = wordTranslations;
	}

	public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
	public event EventHandler<ImportedEventArgs>? Imported;
	public event EventHandler<ImportEventArgs>? LogMessage;

	public int GetToBeImportedCount()
	{
		return _wordTranslations.Count();
	}

	public async Task Import(HttpClient httpClient, string languageCode, int dictionaryId,
		Dictionary<string, Dictionary<int, string>> originalIdsToImportIdsMap)
	{
		WordTranslation[] wordTranslationsInLanguage = _wordTranslations
			.Where(q => q.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();

		LogMessage?.Invoke(this, new ImportEventArgs()
		{
			LogMessage =
				$"Importing {wordTranslationsInLanguage.Count()} Word Translations for Language {languageCode}",
			Indentation = 4
		});


		foreach (WordTranslation translation in _wordTranslations)
		{
			DateTime startTimeStamp = DateTime.Now;
			
			LogMessage?.Invoke(this,new ImportEventArgs()
			{
				LogMessage = $"{translation.TheTranslation}",
				Indentation = 5
			});
			
			try
			{
				// if word is already present, get the existing ID, otherwise, create new for language
				int? fromWordId = await GetWordByOriginalId(httpClient, translation.FromWordId, languageCode);
				if (!fromWordId.HasValue)
					throw new ImportException(
						$"Original Word with ID {translation.FromWordId} not already in imported corpus, no translation can be made");
				
				int? toWordId = await GetWordInDictionary(httpClient,translation.TheTranslation, translation.LanguageCode, dictionaryId);
				if (!toWordId.HasValue)
				{
					// word not already imported, so create it
					CreateWordResult createWordResult = await PostWordToTarget(httpClient, translation.TheTranslation, translation.CreatedAt, translation.CreatedByUserName, dictionaryId,
						translation.Id);
					toWordId = createWordResult.WordId;
				}

				CreateWordTranslationResult createWordTranslationResult =
					await PostTranslationBetweenWords(httpClient, fromWordId.Value, toWordId.Value, dictionaryId, translation.CreatedAt, translation.CreatedByUserName);
				Imported?.Invoke(this,new ImportedEventArgs()
				{
					LanguageCode = languageCode,
					CurrentItem = translation.TheTranslation,
					IsSuccess = true,
					SourceId = translation.Id
				});
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync();
				await Console.Error.WriteLineAsync($"Failed to import Word Translation '{translation.TheTranslation}'");
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
					CurrentItem = translation.TheTranslation,
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

	private async Task<int?> GetWordInDictionary(HttpClient httpClient, string word, string languageCode, int dictionaryId)
	{
		string url = $"/api/v4/words?word={word}&dictionaryId={dictionaryId}";
		HttpResponseMessage response = await httpClient.GetAsync(url);
		if (response.StatusCode == HttpStatusCode.NotFound)
			throw new InvalidOperationException($"Cannot resolve imported Word Translation for Word '{word}'");

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"Error resolving imported Word Translation for Word '{word}'");
		}

		GetWordsResult? getWordsResult = await response.Content.ReadFromJsonAsync<GetWordsResult>();
		if (getWordsResult == null) throw new NullReferenceException("getWordsResult");

		IEnumerable<GetWordResultItem> matchingLanguage=getWordsResult.Results.Where(q =>
			!string.IsNullOrWhiteSpace(q.IetfLanguageTag) &&
			q.IetfLanguageTag.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();
		
		switch (matchingLanguage.Count())
		{
			case 0:
				// word hasn't been previously imported
				return null;
			case 1:
				return getWordsResult.Results.Single().Id;
			default:
				// ambiguous result. 
				throw new ImportException(
					$"Expected zero or one results getting matching Words for '{word}' in Dictionary ID {dictionaryId} but got {matchingLanguage.Count()} which is ambiguous");
		}

	}

	private async Task<int?> GetWordByOriginalId(HttpClient httpClient, int originalWordId, string languageCode)
	{
		string externalId = $"Taggloo2-Word-{originalWordId}";
		string url = $"/api/v4/words?externalId={externalId}";
		HttpResponseMessage response = await httpClient.GetAsync(url);
		if (response.StatusCode == HttpStatusCode.NotFound)
			throw new InvalidOperationException($"Cannot resolve imported Word for External ID {externalId}");

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"Error resolving imported Word for External ID {externalId}");
		}

		GetWordsResult? getWordsResult = await response.Content.ReadFromJsonAsync<GetWordsResult>();
		if (getWordsResult == null) throw new NullReferenceException("getWordsResult");

		IEnumerable<GetWordResultItem> matchingLanguage=getWordsResult.Results.Where(q =>
			!string.IsNullOrWhiteSpace(q.IetfLanguageTag) &&
			!q.IetfLanguageTag.Equals(languageCode, StringComparison.OrdinalIgnoreCase)); // not this language implies the other language
		
		switch (matchingLanguage.Count())
		{
			case 0:
				// word hasn't been previously imported
				return null;
			case 1:
				return getWordsResult.Results.Single().Id;
			default:
				// ambiguous result. 
				return null;
		}

	}

	private async Task<CreateWordTranslationResult> PostTranslationBetweenWords(HttpClient httpClient, 
		int fromWordId, 
		int toWordId, 
		int dictionaryId,
		DateTime createdAt,
		string createdBy)
	{
		string url = "/api/v4/translations/word";
		CreateWordTranslation createWordTranslation = new CreateWordTranslation()
		{
			DictionaryId = dictionaryId,
			FromWordId = fromWordId,
			ToWordId = toWordId,
			CreatedAt = createdAt, //TODO is this date correct?
			CreatedByUserName = createdBy
		};

		HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createWordTranslation);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}

		CreateWordTranslationResult? createWordTranslationResult =
			await response.Content.ReadFromJsonAsync<CreateWordTranslationResult>();
		return createWordTranslationResult!;
	}

	private async Task<CreateWordResult> PostWordToTarget(HttpClient httpClient, string word, DateTime createdAt, string createdBy,
		int dictionaryId, int originalTranslationId)
	{
		string url = "/api/v4/words";
		CreateWord createWord = new CreateWord()
		{
			Word = word,
			CreatedAt = createdAt,
			CreatedByUserName = createdBy,
			DictionaryId = dictionaryId,
			ExternalId = $"Taggloo2-TranslatedWord-{originalTranslationId}"
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