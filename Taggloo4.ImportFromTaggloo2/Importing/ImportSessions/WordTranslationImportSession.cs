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

	public string ContentTypeKey { get; } = "WordTranslation";
	
	public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
	public event EventHandler<ImportedEventArgs>? Imported;
	public event EventHandler<ImportEventArgs>? LogMessage;

	public int GetToBeImportedCount()
	{
		return _wordTranslations.Count();
	}

	public async Task ImportAcrossDictionariesAsync(HttpClient httpClient, string languageCode1, int dictionary1Id, string languageCode2,
		int dictionary2Id)
	{
		await ImportWithinDictionaryAsync(httpClient, languageCode1, dictionary1Id, languageCode2, dictionary2Id);
		await ImportWithinDictionaryAsync(httpClient, languageCode2, dictionary2Id, languageCode1, dictionary1Id);
	}

	private async Task ImportWithinDictionaryAsync(HttpClient httpClient, string fromLanguageCode, int fromDictionaryId, string toLanguageCode, int toDictionaryId)
	{
		WordTranslation[] wordTranslationsInLanguage = _wordTranslations
			.Where(q => q.FromLanguageCode.Equals(fromLanguageCode, StringComparison.OrdinalIgnoreCase)).ToArray();

		LogMessage?.Invoke(this, new ImportEventArgs()
		{
			LogMessage =
				$"Importing {wordTranslationsInLanguage.Count()} Word Translations for Language {fromLanguageCode}",
			Indentation = 4
		});

		int numberOfWordsCreated = 0;
		int numberOfWordseferenced = 0;
		
		foreach (WordTranslation translation in wordTranslationsInLanguage)
		{
			DateTime startTimeStamp = DateTime.Now;
			
			LogMessage?.Invoke(this,new ImportEventArgs()
			{
				LogMessage = $"{translation.TheTranslation}",
				Indentation = 5
			});
			
			try
			{
				// does word already exist? In which case, we can link to that
				int? fromWordId = await GetWordInDictionary(httpClient, translation.FromWord.Trim(), fromLanguageCode,
					fromDictionaryId);

				if (!fromWordId.HasValue)
				{
					// word does not exist, so create new
					CreateWordResult postWordToTargetResult = await PostWordToTarget(httpClient, translation.FromWord,
						translation.CreatedAt, translation.CreatedByUserName, fromDictionaryId, translation.Id);
					fromWordId = postWordToTargetResult.WordId;
					LogMessage?.Invoke(this,new ImportEventArgs()
					{
						LogMessage = $"New Word ID {fromWordId} created for Dictionary ID {fromDictionaryId}",
						Indentation = 6
					});
					numberOfWordsCreated++;
				}
				else
				{
					numberOfWordseferenced++;
				}

				if (!translation.ToLanguageCode.Equals(toLanguageCode, StringComparison.OrdinalIgnoreCase))
				{
					throw new ImportException($"Translation language does not equal target Language");
				}

				int? toWordId = await GetWordInDictionary(httpClient,translation.TheTranslation.Trim(), translation.ToLanguageCode, toDictionaryId);
				if (!toWordId.HasValue)
				{
					// word not already imported, so create it
					CreateWordResult createWordResult = await PostWordToTarget(httpClient,
						translation.TheTranslation, translation.CreatedAt, translation.CreatedByUserName,
						toDictionaryId,
						translation.Id);
					toWordId = createWordResult.WordId;
					LogMessage?.Invoke(this, new ImportEventArgs()
					{
						LogMessage = $"New Word ID {fromWordId} created for Dictionary ID {toDictionaryId}",
						Indentation = 6
					});
				}

				_ = await PostTranslationBetweenWords(httpClient, fromWordId.Value, toWordId.Value, fromDictionaryId, translation.CreatedAt, translation.CreatedByUserName);
				Imported?.Invoke(this,new ImportedEventArgs()
				{
					LanguageCode = fromLanguageCode,
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
					LanguageCode = fromLanguageCode,
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
		
		LogMessage?.Invoke(this,new ImportEventArgs()
		{
			LogMessage = $"Referenced {numberOfWordseferenced} existing Words, created {numberOfWordsCreated} new Words",
			Indentation = 5
		});

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

	private async Task<CreateWordTranslationResult> PostTranslationBetweenWords(HttpClient httpClient, 
		int fromWordId, 
		int toWordId, 
		int dictionaryId,
		DateTime createdAt,
		string createdBy)
	{
		string url = "/api/v4/wordtranslations";
		CreateWordTranslation createWordTranslation = new CreateWordTranslation()
		{
			DictionaryId = dictionaryId,
			FromWordId = fromWordId,
			ToWordId = toWordId,
			CreatedAt = createdAt, 
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
			Word = word.Trim(),
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