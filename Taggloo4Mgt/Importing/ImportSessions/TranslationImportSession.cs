using Taggloo4Mgt.Importing.Importers;

namespace Taggloo4Mgt.Importing.ImportSessions;

public class TranslationImportSession : IImportSession
{
    public int GetToBeImportedCount()
    {
        throw new NotImplementedException();
    }

    public Task Import(HttpClient httpClient, string languageCode)
    {
        	// get translations
		// Log($"\t\t\t\t\t\t{translations.Count()} Translations");
		//
		// foreach (WordTranslation translation in translations)
		// {
		// 	Log($"\t\t\t\t\t\t\t{translation.TheTranslation} ({translation.LanguageCode})");
		// 	if (!dictionariesAtTargetDictionary.ContainsKey(translation.LanguageCode))
		// 	{
		// 		CreateDictionaryResult createOtherDictionaryResult =
		// 			await CreateDictionaryForLanguage(httpClient, translation.LanguageCode);
		// 		Log($"\t\t\t\t\t\t\tDictionary ID {createOtherDictionaryResult.Id} created for {translation.LanguageCode}");
		// 		dictionariesAtTargetDictionary.Add(translation.LanguageCode,createOtherDictionaryResult.Id);
		// 	}
		//
		// 	// post word
		// 	// we need to see if the word already exists. If it does, do not create another word, instead link the existing
		// 	GetWordsResult getOtherWordResult = await GetWord(httpClient,
		// 		translation.TheTranslation, dictionariesAtTargetDictionary[translation.LanguageCode]);
		// 	int otherWordId;
		// 	if (getOtherWordResult.TotalItemsCount == 0)
		// 	{
		// 		// the word doesn't already exist
		// 		CreateWordResult otherCreateWordResult = await PostWordToTarget(httpClient,
		// 			translation.TheTranslation,
		// 			dictionariesAtTargetDictionary[translation.LanguageCode]);
		// 		otherWordId = otherCreateWordResult.Id;
		//
		// 		// this will set the creation meta data for the target word to that of the Translation
		// 		await UpdateWordAtTargetWithMetaData(httpClient, otherWordId, translation.CreatedByUserName,translation.CreatedAt);
		// 		Log($"\t\t\t\t\t\t\tWord ID {otherWordId} created");
		//
		// 	}
		// 	else
		// 	{
		// 		otherWordId = getOtherWordResult.Results.First().Id;
		// 		Log($"\t\t\t\t\t\t\tWord ID {otherWordId} already exists, adding translation");
		// 	}
		// 	
		// 	// post translation
		// 	CreateWordTranslationResult? createWordTranslationResult = await PostTranslationBetweenWords(httpClient,createWordResult.Id,otherWordId,dictionariesAtTargetDictionary[languageCode]);
		//
		// 	await UpdateWordTranslationAtTargetWithMetaData(httpClient,
		// 		createWordTranslationResult!.Id, translation.CreatedByUserName,
		// 		translation.CreatedAt);
		// 	Log($"\t\t\t\t\t\t\tTranslation ID {createWordTranslationResult.Id} created");
		//
        
    }

    public event EventHandler<ImportEventArgs>? LogMessage;
    public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
    public event EventHandler<ImportedEventArgs>? Imported;
}