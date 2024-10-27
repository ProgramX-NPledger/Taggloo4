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

	public string ContentTypeKey { get; } = "PhraseTranslation";

	public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
	public event EventHandler<ImportedEventArgs>? Imported;
	public event EventHandler<ImportEventArgs>? LogMessage;

	public int GetToBeImportedCount()
	{
		return _phraseTranslations.Count();
	}

	public async Task ImportAcrossDictionariesAsync(HttpClient httpClient, string languageCode1, int dictionary1Id, string languageCode2,
		int dictionary2Id)
	{
		await ImportWithinDictionaryAsync(httpClient, languageCode1, dictionary1Id, languageCode2, dictionary2Id);
		await ImportWithinDictionaryAsync(httpClient, languageCode2, dictionary2Id, languageCode1, dictionary1Id);
	}

	private async Task ImportWithinDictionaryAsync(HttpClient httpClient, string fromLanguageCode, int fromDictionaryId, string toLanguageCode, int toDictionaryId)
	{
		PhraseTranslation[] phraseTranslationsInLanguage = _phraseTranslations
			.Where(q => q.LanguageCode.Equals(toLanguageCode, StringComparison.OrdinalIgnoreCase)).ToArray();

		LogMessage?.Invoke(this, new ImportEventArgs()
		{
			LogMessage =
				$"Importing {phraseTranslationsInLanguage.Count()} Phrase Translations for Language {fromLanguageCode}",
			Indentation = 4
		});


		foreach (PhraseTranslation translation in phraseTranslationsInLanguage)
		{
			DateTime startTimeStamp = DateTime.Now;
			
			LogMessage?.Invoke(this,new ImportEventArgs()
			{
				LogMessage = $"{translation.Translation}",
				Indentation = 5
			});

			try
			{

				// try and get ID of Phrase using alternative lookup method
				int? fromPhraseId = await GetPhraseInDictionary(httpClient, translation.FromPhrase.Trim(), fromLanguageCode,
					fromDictionaryId);

				if (!fromPhraseId.HasValue)
				{
					CreatePhraseResult postPhraseToTargetResult = await PostPhraseToTarget(httpClient,
						translation.FromPhrase,
						translation.CreatedAt, translation.CreatedByUserName, fromDictionaryId, translation.Id,fromLanguageCode);
					fromPhraseId = postPhraseToTargetResult.PhraseId;
					LogMessage?.Invoke(this, new ImportEventArgs()
					{
						LogMessage = $"New Phrase ID {fromPhraseId} created for Dictionary ID {fromDictionaryId}",
						Indentation = 6
					});
				}
				
				int? toPhraseId = await GetPhraseInDictionary(httpClient,translation.Translation, translation.LanguageCode, toDictionaryId);
				if (!toPhraseId.HasValue)
				{
					// phrase not already imported, so create it
					CreatePhraseResult createPhraseResult = await PostPhraseToTarget(httpClient, translation.Translation, translation.CreatedAt, translation.CreatedByUserName, fromDictionaryId,
						translation.Id,translation.LanguageCode);
					toPhraseId = createPhraseResult.PhraseId;
					LogMessage?.Invoke(this,new ImportEventArgs()
					{
						LogMessage = $"New Phrase ID {fromPhraseId} created for Dictionary ID {toDictionaryId}",
						Indentation = 6
					});
				}

				_ = await PostTranslationBetweenPhrases(httpClient, fromPhraseId.Value, toPhraseId.Value, fromDictionaryId, translation.CreatedAt, translation.CreatedByUserName);
				Imported?.Invoke(this,new ImportedEventArgs()
				{
					LanguageCode = fromLanguageCode,
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
					LanguageCode = fromLanguageCode,
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

	private async Task<CreatePhraseResult> PostPhraseToTarget(HttpClient httpClient, string phrase, DateTime createdAt, string createdBy, int dictionaryId, int originalTranslationId, string ietfLanguageTag)
	{
		string url = "/api/v4/phrases";
		CreatePhrase createPhrase = new CreatePhrase()
		{
			Phrase = phrase,
			CreatedAt = createdAt,
			CreatedByUserName = createdBy,
			DictionaryId = dictionaryId,
			ExternalId = $"Taggloo2-TranslatedPhrase-{originalTranslationId}",
			IetfLanguageTag = ietfLanguageTag
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
			// if getting by Dictionary didn't work, try again with language - the phrase may be in another dictionary for the same language
			url = $"/api/v4/phrases?phrase={phrase}&ietfLanguageTag={languageCode}";
			response = await httpClient.GetAsync(url);
			if (response.StatusCode == HttpStatusCode.NotFound)
				throw new InvalidOperationException($"Cannot resolve imported Phrase for Phrase '{phrase}'");

			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException(
					$"Error resolving imported Phrase for Phrase '{phrase}'");
			}
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
			default:
				// ambiguous result. 
				throw new ImportException(
					$"Expected zero or one results getting matching Phrases for '{phrase}' in Dictionary ID {dictionaryId} but got {matchingLanguage.Count()} which is ambiguous");
		}

	}

	// private async Task<int?> GetPhraseByOriginalId(HttpClient httpClient, int originalPhraseId, string languageCode)
	// {
	// 	string externalId = $"Taggloo2-Phrase-{originalPhraseId}";
	// 	string url = $"/api/v4/phrases?externalId={externalId}";
	// 	HttpResponseMessage response = await httpClient.GetAsync(url);
	// 	if (response.StatusCode == HttpStatusCode.NotFound)
	// 		throw new InvalidOperationException($"Cannot resolve imported Phrase for External ID {externalId}");
	//
	// 	if (!response.IsSuccessStatusCode)
	// 	{
	// 		throw new InvalidOperationException(
	// 			$"Error resolving imported Phrase for External ID {externalId}");
	// 	}
	//
	// 	GetPhrasesResult? getPhrasesResult = await response.Content.ReadFromJsonAsync<GetPhrasesResult>();
	// 	if (getPhrasesResult == null) throw new NullReferenceException("getPhrasesResult");
	//
	// 	IEnumerable<GetPhraseResultItem> matchingLanguage=getPhrasesResult.Results.Where(q =>
	// 		!string.IsNullOrWhiteSpace(q.IetfLanguageTag) &&
	// 		!q.IetfLanguageTag.Equals(languageCode, StringComparison.OrdinalIgnoreCase)); // not this language implies the other language
	// 	
	// 	switch (matchingLanguage.Count())
	// 	{
	// 		case 0:
	// 			// phrase hasn't been previously imported
	// 			return null;
	// 		case 1:
	// 			return getPhrasesResult.Results.Single().Id;
	// 		default:
	// 			// ambiguous result. 
	// 			return null;
	// 	}
	//
	// }

	private async Task<CreatePhraseTranslationResult?> PostTranslationBetweenPhrases(HttpClient httpClient, 
		int fromPhraseId, 
		int toPhraseId, 
		int dictionaryId,
		DateTime createdAt,
		string createdBy)
	{
		string url = "/api/v4/phrasetranslations";
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

	
	
	
	
	
}