using System.Net;
using System.Net.Http.Json;
using Taggloo4.Dto;
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
		Dictionary<string, Dictionary<int, Guid>> originalIdsToImportIdsMap)
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
			// get the ID of the Phrase
			if (!originalIdsToImportIdsMap.ContainsKey(nameof(PhraseImportSession)))
			{
				throw new InvalidOperationException(
					$"Dictionary of previously imported Phrases does not include for type {nameof(PhraseImportSession)}");
			}

			if (!originalIdsToImportIdsMap[nameof(WordImportSession)].ContainsKey(translation.FromPhraseId))
			{
				// the original phrase was not previously created, which is a problem

			}
			else
			{
				Guid importGuidOfFromPhrase =
					originalIdsToImportIdsMap[nameof(PhraseImportSession)][translation.FromPhraseId];
				Guid importGuidOfToPhrase =
					originalIdsToImportIdsMap[nameof(TranslatedPhraseImportSession)][translation.ToPhraseId];
				
				// need to get the true IDs of the words to import
				int fromPhraseId = await GetPhraseByImportGuid(httpClient, importGuidOfFromPhrase);
				int toPhraseId = await GetPhraseByImportGuid(httpClient, importGuidOfToPhrase);
				
				// post translation
				CreatePhraseTranslationResult? createPhraseTranslationResult = await PostTranslationBetweenPhrases(httpClient,
					fromPhraseId, toPhraseId, dictionaryId, translation.CreatedAt, translation.CreatedByUserName);




			}
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

	private async Task<int> GetPhraseByImportGuid(HttpClient httpClient, Guid importGuidOfFromPhrase)
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