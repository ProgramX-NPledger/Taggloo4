using System.Net;
using System.Net.Http.Json;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;
using Taggloo4Mgt.Importing.Importers;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.ImportSessions;

public class PhraseTranslationImportSession : IImportSession
{
	private readonly IEnumerable<PhraseTranslation> _phraseTranslations;

	public PhraseTranslationImportSession(IEnumerable<PhraseTranslation> phraseTranslations)
	{
		_phraseTranslations = phraseTranslations;
	}

	public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
	public event EventHandler<ImportedEventArgs>? Imported;
	public event EventHandler<ImportEventArgs>? LogMessage;

	public int GetToBeImportedCount()
	{
		return _phraseTranslations.Count();
	}

	public async Task Import(HttpClient httpClient, string languageCode, int dictionaryId,
		Dictionary<string, Dictionary<int, string>> originalIdsToImportIdsMap)
	{
		PhraseTranslation[] phraseTranslationsInLanguage = _phraseTranslations
			.Where(q => q.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();

		LogMessage?.Invoke(this, new ImportEventArgs()
		{
			LogMessage =
				$"Importing {phraseTranslationsInLanguage.Count()} Phrase Translations for Language {languageCode}",
			Indentation = 4
		});


		foreach (PhraseTranslation translation in _phraseTranslations)
		{
			DateTime startTimeStamp = DateTime.Now;
			
			LogMessage?.Invoke(this,new ImportEventArgs()
			{
				LogMessage = $"{translation.Translation}",
				Indentation = 5
			});

			try
			{
				// if phrase is already present, get the existing ID, otherwise, create new for language
				int? fromPhraseId = await GetPhraseByOriginalId(httpClient, translation.FromPhraseId, translation.LanguageCode);
				if (!fromPhraseId.HasValue)
					throw new ImportException(
						$"Original Phrase with ID {translation.FromPhraseId} not already in imported corpus, no translation can be made");
				
				int? toPhraseId = await GetPhraseInDictionary(httpClient,translation.Translation, translation.LanguageCode, dictionaryId);
				if (!toPhraseId.HasValue)
				{
					// phrase not already imported, so create it
					CreatePhraseResult createPhraseResult = await PostPhraseToTarget(httpClient, translation.Translation, translation.CreatedAt, translation.CreatedByUserName, dictionaryId,
						translation.Id);
					toPhraseId = createPhraseResult.PhraseId;
				}

				CreatePhraseTranslationResult createPhraseTranslationResult =
					await PostTranslationBetweenPhrases(httpClient, fromPhraseId.Value, toPhraseId.Value, dictionaryId, translation.CreatedAt, translation.CreatedByUserName);
				Imported?.Invoke(this,new ImportedEventArgs()
				{
					LanguageCode = languageCode,
					CurrentItem = translation.Translation,
					IsSuccess = true,
					SourceId = translation.Id
				});
			}
			catch (Exception ex)
			{
				await Console.Error.WriteLineAsync();
				await Console.Error.WriteLineAsync($"Failed to import Phrase Translation '{translation.Translation}'");
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
					CurrentItem = translation.Translation,
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

	private async Task<CreatePhraseResult> PostPhraseToTarget(HttpClient httpClient, string phrase, DateTime createdAt, string createdBy, int dictionaryId, int originalTranslationId)
	{
		string url = "/api/v4/phrases";
		CreatePhrase createPhrase = new CreatePhrase()
		{
			Phrase = phrase,
			CreatedAt = createdAt,
			CreatedByUserName = createdBy,
			DictionaryId = dictionaryId,
			ExternalId = $"Taggloo2/TranslatedPhrase/{originalTranslationId}"
		};
		
		HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createPhrase);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}
		
		CreatePhraseResult? createPhraseResult =
			await response.Content.ReadFromJsonAsync<CreatePhraseResult>();
		if (createPhraseResult == null) throw new NullReferenceException("createPhraseResult");
		return createPhraseResult; 
	}

	private async Task<int?> GetPhraseInDictionary(HttpClient httpClient, string phrase, string languageCode, int dictionaryId)
	{
		string url = $"/api/v4/phrases?phrase={phrase}&dictionaryId={dictionaryId}";
		HttpResponseMessage response = await httpClient.GetAsync(url);
		if (response.StatusCode == HttpStatusCode.NotFound)
			throw new InvalidOperationException($"Cannot resolve imported Phrase for Phrase '{phrase}'");

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"Error resolving imported Phrase for Phrase '{phrase}'");
		}

		GetPhrasesResult? getPhrasesResult = await response.Content.ReadFromJsonAsync<GetPhrasesResult>();
		if (getPhrasesResult == null) throw new NullReferenceException("getPhrasesResult");

		IEnumerable<GetPhraseResultItem> matchingLanguage=getPhrasesResult.Results.Where(q =>
			!string.IsNullOrWhiteSpace(q.IetfLanguageTag) &&
			q.IetfLanguageTag.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();
		
		switch (matchingLanguage.Count())
		{
			case 0:
				// phrase hasn't been previously imported
				return null;
			case 1:
				return getPhrasesResult.Results.Single().Id;
				break;
			default:
				// ambiguous result. 
				throw new ImportException(
					$"Expected zero or one results getting matching Phrases for '{phrase}' in Dictionary ID {dictionaryId} but got {matchingLanguage.Count()} which is ambiguous");
		}

	}

	private async Task<int?> GetPhraseByOriginalId(HttpClient httpClient, int originalPhraseId, string languageCode)
	{
		string externalId = $"Taggloo2/Phrase/{originalPhraseId}";
		string url = $"/api/v4/phrases/{externalId}/externalId";
		HttpResponseMessage response = await httpClient.GetAsync(url);
		if (response.StatusCode == HttpStatusCode.NotFound)
			throw new InvalidOperationException($"Cannot resolve imported Phrase for External ID {externalId}");

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"Error resolving imported Word for External ID {externalId}");
		}

		GetPhrasesResult? getPhrasesResult = await response.Content.ReadFromJsonAsync<GetPhrasesResult>();
		if (getPhrasesResult == null) throw new NullReferenceException("getPhrasesResult");

		IEnumerable<GetPhraseResultItem> matchingLanguage=getPhrasesResult.Results.Where(q =>
			!string.IsNullOrWhiteSpace(q.IetfLanguageTag) &&
			q.IetfLanguageTag.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
		
		switch (matchingLanguage.Count())
		{
			case 0:
				// phrase hasn't been previously imported
				return null;
			case 1:
				return getPhrasesResult.Results.Single().Id;
				break;
			default:
				// ambiguous result. 
				return null;
		}

	}

	private async Task<CreatePhraseTranslationResult?> PostTranslationBetweenPhrases(HttpClient httpClient, 
		int fromPhraseId, 
		int toPhraseId, 
		int dictionaryId,
		DateTime createdAt,
		string createdBy)
	{
		string url = "/api/v4/translations/phrase";
		CreatePhraseTranslation createPhraseTranslation = new CreatePhraseTranslation()
		{
			DictionaryId = dictionaryId,
			FromPhraseId = fromPhraseId,
			ToPhraseId = toPhraseId,
			CreatedAt = createdAt,
			CreatedByUserName = createdBy
		};

		HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createPhraseTranslation);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}

		CreatePhraseTranslationResult? createPhraseTranslationResult =
			await response.Content.ReadFromJsonAsync<CreatePhraseTranslationResult>();
		return createPhraseTranslationResult;
	}

	
	
	
	
	
	
	
	private async Task<int> FindTranslatedPhrase(HttpClient httpClient, string translationTheTranslation, string languageCode)
	{
		string url = $"/api/v4/phrases?phrase={translationTheTranslation}"; // this will return across Dictionaries
		HttpResponseMessage response = await httpClient.GetAsync(url);

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"Error finding matching Phrase for '{translationTheTranslation}'");
		}

		GetPhrasesResult? getPhrasesResult = await response.Content.ReadFromJsonAsync<GetPhrasesResult>();
		if (getPhrasesResult == null) throw new NullReferenceException("getPhrasesResult");
		
		// get for the other Dictionary (there are only two languages, so the other must be the right one)
		GetPhraseResultItem[] phrasesInOtherLanguage= getPhrasesResult.Results.Where(q => !q.IetfLanguageTag.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (phrasesInOtherLanguage.Count() == 0)
		{
			throw new InvalidOperationException($"Failed to find matching Phrase for '{translationTheTranslation} which is not in Language {languageCode}");
		}
		
		return phrasesInOtherLanguage.First().Id;
	}

	private async Task<int> GetPhraseByImportGuid(HttpClient httpClient, string importGuidOfFromPhrase)
	{
		string url = $"/api/v4/phrases/{importGuidOfFromPhrase}/importguid";
		HttpResponseMessage response = await httpClient.GetAsync(url);
		if (response.StatusCode == HttpStatusCode.NotFound)
			throw new InvalidOperationException($"Cannot resolve imported Phrase for Import Guid {importGuidOfFromPhrase}");

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"Error resolving imported Phrase for Import Guid {importGuidOfFromPhrase}");
		}

		GetPhraseResultItem? getPhraseResultItem = await response.Content.ReadFromJsonAsync<GetPhraseResultItem>();
		if (getPhraseResultItem == null) throw new NullReferenceException("getPhraseResultItem");
		return getPhraseResultItem.Id;
	}
}