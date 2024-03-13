using System.Net;
using System.Net.Http.Json;
using Taggloo4.Dto;
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
			// get the ID of the Word
			if (!originalIdsToImportIdsMap.ContainsKey(nameof(WordImportSession)))
			{
				throw new InvalidOperationException(
					$"Dictionary of previously imported Words does not include for type {nameof(WordImportSession)}");
			}

			if (!originalIdsToImportIdsMap[nameof(WordImportSession)].ContainsKey(translation.FromWordId))
			{
				// the original word was not previously created, which is a problem

			}
			else
			{
				string importGuidOfFromWord =
					originalIdsToImportIdsMap[nameof(WordImportSession)][translation.FromWordId];

				// need to get the true IDs of the words to import
				int fromWordId = await GetWordByImportGuid(httpClient, importGuidOfFromWord);
				int toWordId = await FindTranslatedWord(httpClient, translation.TheTranslation, languageCode);

				// post translation
				CreateWordTranslationResult? createWordTranslationResult = await PostTranslationBetweenWords(httpClient,
					fromWordId, toWordId, dictionaryId, translation.CreatedAt, translation.CreatedByUserName);




			}
		}


	}

	private async Task<CreateWordTranslationResult?> PostTranslationBetweenWords(HttpClient httpClient, 
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
		return createWordTranslationResult;
	}

	
	private async Task<int> FindTranslatedWord(HttpClient httpClient, string translationTheTranslation, string languageCode)
	{
		string url = $"/api/v4/words?word={translationTheTranslation}"; // this will return across Dictionaries
		HttpResponseMessage response = await httpClient.GetAsync(url);

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"Error finding matching Word for '{translationTheTranslation}'");
		}

		GetWordsResult? getWordsResult = await response.Content.ReadFromJsonAsync<GetWordsResult>();
		if (getWordsResult == null) throw new NullReferenceException("getWordsResult");
		
		// get for the other Dictionary (there are only two languages, so the other must be the right one)
		GetWordResultItem[] wordsInOtherLanguage= getWordsResult.Results.Where(q => !q.IetfLanguageTag.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (wordsInOtherLanguage.Count() == 0)
		{
			throw new InvalidOperationException($"Failed to find matching Word for '{translationTheTranslation} which is not in Language {languageCode}");
		}
		
		return wordsInOtherLanguage.First().Id;
	}

	private async Task<int> GetWordByImportGuid(HttpClient httpClient, string importGuidOfFromWord)
	{
		string url = $"/api/v4/words/{importGuidOfFromWord}/importguid";
		HttpResponseMessage response = await httpClient.GetAsync(url);
		if (response.StatusCode == HttpStatusCode.NotFound)
			throw new InvalidOperationException($"Cannot resolve imported Word for Import Guid {importGuidOfFromWord}");

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"Error resolving imported Word for Import Guid {importGuidOfFromWord}");
		}

		GetWordResultItem? getWordResultItem = await response.Content.ReadFromJsonAsync<GetWordResultItem>();
		if (getWordResultItem == null) throw new NullReferenceException("getWordResultItem");
		return getWordResultItem.Id;
	}
}