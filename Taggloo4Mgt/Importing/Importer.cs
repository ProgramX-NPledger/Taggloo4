﻿using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Loader;
using System.Text;
using Dapper;
using Taggloo4.Dto;
using Taggloo4Mgt.Importing.Importers;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing;
public class Importer : ApiClientBase
{
	private readonly ImportOptions _importOptions;
	private readonly string? _logFileName;
	private readonly IList<int> _millisecondsBetweenWords = new List<int>();
	
	public Importer(ImportOptions importOptions)
	{
		_importOptions = importOptions;
		_logFileName = CreateRandomLogFileName();
		

	}

	private string CreateRandomLogFileName()
	{
		return $"log{DateTime.Now:yyyyMMddHHmm}.txt";
	}

	public async Task<int> Process()
	{
		if (_importOptions.Log) Console.WriteLine($"Logging to {_logFileName}");
		Log($"Started processing at {DateTime.Now}");
		Log($"Connect to SQL Server {_importOptions.SourceConnectionString}");
		// connect to SQL Server
		await using (SqlConnection sqlConnection = ConnectToSqlServer())
		{
			Log("\tOk");
			Log($"Connect to API at {_importOptions.Url}");
			using (HttpClient httpClient = CreateHttpClient(_importOptions.Url))
			{
				Log("\tOk");

				Log($"\tLog in to API as {_importOptions.UserName}");
				LoginUserResult loginUserResult = await ConnectToApi(httpClient);
				if (loginUserResult == null) throw new InvalidOperationException("Failed to log in to API");
				Log("\t\tOk");

				httpClient.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", loginUserResult.Token);

				IEnumerable<string> sourceLanguageCodes =
					await GetLanguagesFromSourceAndEnforceAtTarget(sqlConnection, httpClient);

				if (_importOptions.ResetAllDictionaries)
				{
					// reset all existing dictionaries for languages prior to import
					await DeleteAllDictionariesForLanguage(httpClient, sourceLanguageCodes);
				}

				IEnumerable<IImporter> importers = ImporterFactory.GetImporters();

				// if no import types specified, select all
				if (!_importOptions.ImportTypes.Any())
					_importOptions.ImportTypes = importers.Select(q => q.Key);
				Log($"\t\tImporting: {string.Join(", ", _importOptions.ImportTypes.ToArray())}");

				// prepare all imports
				List<IImportSession> importSessions = new List<IImportSession>();
				int toBeImportedCount = 0;
				int totalImportedCount = 0;
				List<double> millisecondsBetweenOperations = new List<double>();
				foreach (IImporter importer in importers)
				{
					IImportSession importSession = await importer.CreateSession(sqlConnection);
					toBeImportedCount += importSession.GetToBeImportedCount();
					importSession.UpdateMetrics += (sender, e) =>
					{
						millisecondsBetweenOperations.Add(e.MillisecondsBetweenImports);
					};
					importSession.LogMessage += (sender, e) =>
					{
						StringBuilder tabsSb = new StringBuilder();
						for (int i = 0; i < e.Indentation; i++)
						{
							tabsSb.Append("\t");
						}

						Log($"{tabsSb.ToString()}{e.LogMessage}");
					};
					importSession.Imported += (sender, e) =>
					{
						UpdateProgressBar(++totalImportedCount, toBeImportedCount, e.LanguageCode, e.CurrentItem);
					};
					importSessions.Add(importSession);
				}

				foreach (string languageCode in sourceLanguageCodes)
				{
					int numberOfImportedItemsForType = 0;
					foreach (IImportSession importSession in importSessions)
					{
						// create a dictionary
						CreateDictionaryResult dictionary =
							await CreateDictionaryForLanguage(httpClient, languageCode, nameof(importSession));
						if ((_importOptions.MaxItemsPerType ?? 0) < numberOfImportedItemsForType)
						{
							numberOfImportedItemsForType++;
							importSession.Import(httpClient, languageCode, dictionary.Id);
						}
					}

				}

			}

		}

		Console.WriteLine();
		
		return 0;

	}


	private async Task<IEnumerable<string>> GetLanguagesFromSourceAndEnforceAtTarget(SqlConnection sqlConnection, HttpClient httpClient)
	{
		// get all languages
		Log("\t\tGet all Languages");
		IEnumerable<string> sourceLanguageCodes = (await GetLanguageCodesFromSource(sqlConnection)).ToArray();
		Log($"\t\t\t{sourceLanguageCodes.Count()} Languages");

		// have to create both languages before we start
		foreach (string languageCode in sourceLanguageCodes)
		{
			if (!await IsLanguageKnown(httpClient,languageCode))
			{
				CreateLanguageResult createLanguageResult=await CreateLanguage(httpClient,languageCode);
				Log($"\t\t\tLanguage {languageCode} is not known at target, created at target");
			}
			else
			{
				Log($"\t\t\tLanguage {languageCode} is already configured at target");
			}
		}

		return sourceLanguageCodes;
	}

	private async Task ImportPhrases(HttpClient httpClient, SqlConnection sqlConnection, string languageCode, int processedCount, int totalProcessedCount, IDictionary<string,int> dictionariesForLanguageDictionary)
	{
		UpdateProgressBar(0, 0, "", "");

		Log($"\t\t\tPhrases: {languageCode}");
			
		// get all words
		IEnumerable<Phrase> phrasesInLanguage = await GetAllPhrasesForLanguage(sqlConnection,languageCode);
		Log($"\t\t\t\t{phrasesInLanguage.Count()} Phrases in Language");
		totalProcessedCount += phrasesInLanguage.Count();
		int phrasesProcessedCount = 0;
		
		// for each Word in source
		foreach (Phrase phraseInLanguage in phrasesInLanguage)
		{
			DateTime startTimeStamp = DateTime.Now;
			
			Log($"\t\t\t\t\t{phraseInLanguage.ThePhrase}");

			if (_importOptions.MaxItemsPerLanguage.HasValue &&
			    _importOptions.MaxItemsPerLanguage.Value < phrasesProcessedCount)
			{
				// already processed enough phrases, so ignore
			}
			else
			{
				try
				{
					phrasesProcessedCount++;
					await ProcessPhrase(httpClient, sqlConnection, phraseInLanguage, dictionariesForLanguageDictionary, languageCode);
				
				}
				catch (Exception ex)
				{
					await Console.Error.WriteLineAsync();
					await Console.Error.WriteLineAsync($"Failed to import Phrase '{phraseInLanguage.ThePhrase}'");
					Exception? exPtr = ex;
					do
					{
						await Console.Error.WriteLineAsync(exPtr.Message);
						Log($"ERROR: {exPtr.GetType().Name}: {exPtr.Message}");
						exPtr = exPtr.InnerException;
					} while (exPtr!=null);
				}
				
			}

			TimeSpan delta = DateTime.Now - startTimeStamp;
			_millisecondsBetweenWords.Add((int)delta.TotalMilliseconds);
			
			
			UpdateProgressBar(processedCount++, totalProcessedCount, languageCode, phraseInLanguage.ThePhrase);
			
			
		}
		
	}

	private async Task ProcessPhrase(HttpClient httpClient, SqlConnection sqlConnection, Phrase phraseInLanguage, IDictionary<string,int> dictionariesAtTargetDictionary, string languageCode)
	{
		
		// does the phrase already exist?
		GetPhrasesResult existingPhrase = await GetPhrase(httpClient, phraseInLanguage.ThePhrase,
			dictionariesAtTargetDictionary[languageCode]);
		if (existingPhrase.Results.Any())
		{
			// phrase already exists, so have to assume translations do too
			Log($"\t\t\t\t\t\tPhrase already exists");
		}
		else
		{
			// post phrase
			CreatePhraseResult createPhraseResult=await PostPhraseToTarget(httpClient, phraseInLanguage.ThePhrase, dictionariesAtTargetDictionary[languageCode]);
			await UpdatePhraseAtTargetWithMetaData(httpClient, createPhraseResult.Id, phraseInLanguage.CreatedByUserName, phraseInLanguage.CreatedTimeStamp);
			Log($"\t\t\t\t\t\tWord ID {createPhraseResult.Id} created");
			
			// get translations
			IEnumerable<PhraseTranslation> translations = await GetTranslationsForPhrase(sqlConnection, phraseInLanguage);
			Log($"\t\t\t\t\t\t{translations.Count()} Translations");
			
			foreach (PhraseTranslation translation in translations)
			{
				Log($"\t\t\t\t\t\t\t{translation.Translation} ({translation.LanguageCode})");
				if (!dictionariesAtTargetDictionary.ContainsKey(translation.LanguageCode))
				{
					CreateDictionaryResult createOtherDictionaryResult =
						await CreateDictionaryForLanguage(httpClient, translation.LanguageCode);
					Log($"\t\t\t\t\t\t\tDictionary ID {createOtherDictionaryResult.Id} created for {translation.LanguageCode}");
					dictionariesAtTargetDictionary.Add(translation.LanguageCode,createOtherDictionaryResult.Id);
				}
			
				// post phrase
				// we need to see if the phrase already exists. If it does, do not create another word, instead link the existing
				GetPhrasesResult getOtherPhraseResult = await GetPhrase(httpClient,
					translation.Translation, dictionariesAtTargetDictionary[translation.LanguageCode]);
				int otherPhraseId;
				if (getOtherPhraseResult.TotalItemsCount == 0)
				{
					// the phrase doesn't already exist
					CreatePhraseResult otherCreatePhraseResult = await PostPhraseToTarget(httpClient,
						translation.Translation,
						dictionariesAtTargetDictionary[translation.LanguageCode]);
					otherPhraseId = otherCreatePhraseResult.Id;

					// this will set the creation meta data for the target word to that of the Translation
					await UpdateWordAtTargetWithMetaData(httpClient, otherPhraseId, translation.CreatedByUserName,translation.CreatedAt);
					Log($"\t\t\t\t\t\t\tPhrase ID {otherPhraseId} created");

				}
				else
				{
					otherPhraseId = getOtherPhraseResult.Results.First().Id;
					Log($"\t\t\t\t\t\t\tPhrase ID {otherPhraseId} already exists, adding translation");
				}
				
				// post translation
				CreatePhraseTranslationResult? createPhraseTranslationResult = await PostTranslationBetweenPhrases(httpClient,createPhraseResult.Id,otherPhraseId,dictionariesAtTargetDictionary[languageCode]);

				await UpdatePhraseTranslationAtTargetWithMetaData(httpClient,
					createPhraseTranslationResult!.Id, translation.CreatedByUserName,
					translation.CreatedAt);
				Log($"\t\t\t\t\t\t\tTranslation ID {createPhraseTranslationResult.Id} created");
				
			}

		
		}
		
	}


	

	private async Task DeleteAllDictionariesForLanguage(HttpClient httpClient, IEnumerable<string> sourceLanguageCodes)
	{
		Log($"\t\t\tConfiguration to delete all Dictionaries at target for source languages {string.Join(", ",sourceLanguageCodes.ToArray())}");
		foreach (string languageCode in sourceLanguageCodes)
		{
			// get all dictionaries for Languages

			string url = $"/api/v4/dictionaries?ietfLanguageTag={languageCode}";
			HttpResponseMessage response = await httpClient.GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				GetDictionariesResult? getDictionariesResult = await response.Content.ReadFromJsonAsync<GetDictionariesResult>();
				if (getDictionariesResult == null)
					throw new InvalidOperationException($"Call to {url} resulted in a null response");

				foreach (GetDictionaryResultItem dictionary in getDictionariesResult.Results)
				{
					Log($"\t\t\t\tDeleting Dictionary ID {dictionary.Id} for Language {languageCode}");
					string deleteUrl = $"/api/v4/dictionaries/{dictionary.Id}";
					HttpResponseMessage deleteResponse = await httpClient.DeleteAsync(deleteUrl);
					if (!deleteResponse.IsSuccessStatusCode)
					{
						throw new InvalidOperationException($"Failed to delete Dictionary ID {dictionary.Id}");
					}
				}
				
			}

			
		}
		

	}


	private void UpdateProgressBar(int wordsProcessed, int totalWords, string languageCode, string theWord)
	{
		
		Console.CursorLeft = 0;

		// ReSharper disable once PossibleLossOfFraction
		double percent = wordsProcessed==0 ? 0 : ((double)wordsProcessed / (double)totalWords) * 100.0;
		double avgSecs = _millisecondsBetweenWords.Any() ? _millisecondsBetweenWords.Average() : 0;
		TimeSpan eta = new TimeSpan(0, 0, 0,0,(int)avgSecs * (totalWords - wordsProcessed));

		int percentageBarWidth = 30;
		double percentageBarFilled = wordsProcessed==0 ? 0 : ((double)wordsProcessed / (double)totalWords) * (double)percentageBarWidth;
		
		for (int i=1; i< percentageBarFilled; i++) Console.Write("#");
		for (int i=(int)percentageBarFilled+1;i<percentageBarWidth; i++) Console.Write("-");

		string s = $" {((int)percent).ToString(CultureInfo.InvariantCulture),3}% ({wordsProcessed}/{totalWords}) {languageCode} {theWord}";
		Console.Write($"{s,-60}ETA: {eta:hh\\:mm\\:ss}");
		

	}

	private void Log(string s)
	{
		if (_importOptions.Log)
		{
			int attempts = 0;

			while (true)
			{
				try
				{
					attempts++;
					File.AppendAllLines(_logFileName!,new string[]
					{
						s
					});
					return;
				}
				catch (Exception e)
				{
					if (e.Message.Contains("The process cannot access the file") && attempts<6)
					{
						// suspect a locking issue, hold off a second before retrying
						Thread.Sleep(1000*attempts);
					}
					else
					{
						Console.WriteLine(e);
						throw;
					}
				}
			}
			
		}
	}

	
	private async Task UpdatePhraseTranslationAtTargetWithMetaData(HttpClient httpClient, int id, string translationCreatedByUserName, DateTime translationCreatedAt)
	{
		string url = $"/api/v4/translations/phrase/{id}";
		UpdatePhraseTranslation updatePhraseTranslation = new UpdatePhraseTranslation()
		{
			CreatedByUserName = translationCreatedByUserName,
			CreatedAt = translationCreatedAt
		};
		
		HttpResponseMessage response = await httpClient.PatchAsJsonAsync(url, updatePhraseTranslation);
		response.EnsureSuccessStatusCode();
	}
	
	private async Task<CreateWordTranslationResult?> PostTranslationBetweenWords(HttpClient httpClient, int fromWordId, int toWordId, int dictionaryId)
	{
		string url = "/api/v4/translations/word";
		CreateWordTranslation createWordTranslation = new CreateWordTranslation()
		{
			DictionaryId = dictionaryId,
			FromWordId = fromWordId,
			ToWordId = toWordId
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
	
	private async Task<CreatePhraseTranslationResult?> PostTranslationBetweenPhrases(HttpClient httpClient, int fromPhraseId, int toPhraseId, int dictionaryId)
	{
		string url = "/api/v4/translations/phrase";
		CreatePhraseTranslation createPhraseTranslation = new CreatePhraseTranslation()
		{
			DictionaryId = dictionaryId,
			FromPhraseId = fromPhraseId,
			ToPhraseId = toPhraseId
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

	private async Task<CreateDictionaryResult> CreateDictionaryForLanguage(HttpClient httpClient, string languageCode, string importType)
	{
		string url = "/api/v4/dictionaries";
		CreateDictionary createDictionary = new CreateDictionary()
		{
			Name = $"Imported from Taggloo2 by {importType}",
			IetfLanguageTag = languageCode,
			SourceUrl = "https://taggloo.im",
			Description = "Imported from SQL Server Taggloo2 database"
		};
		
		HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createDictionary);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}

		CreateDictionaryResult? createDictionaryResult =
			await response.Content.ReadFromJsonAsync<CreateDictionaryResult>();
		return createDictionaryResult!;
	}
	
	
	public async Task<CreateLanguageResult> CreateLanguage(HttpClient httpClient, string languageCode)
	{
		string url = "/api/v4/languages";
		CreateLanguage createLanguage = new CreateLanguage()
		{
			Name = languageCode,
			IetfLanguageTag = languageCode
		};
		
		HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createLanguage);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}

		CreateLanguageResult? createLanguageResult =
			await response.Content.ReadFromJsonAsync<CreateLanguageResult>();
		return createLanguageResult!;
	}
	

	
	private async Task<GetPhrasesResult> GetPhrase(HttpClient httpClient, string phrase, int dictionaryId)
	{
		string url = $"/api/v4/phrases?phrase={phrase}&dictionaryId={dictionaryId}";

		HttpResponseMessage response = await httpClient.GetAsync(url);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}

		GetPhrasesResult? getPhrasesResult = await response.Content.ReadFromJsonAsync<GetPhrasesResult>();
		return getPhrasesResult!;

	}


	

	
	private async Task<IEnumerable<WordTranslation>> GetTranslationsForWord(SqlConnection sqlConnection,
		Word wordInLanguage)
	{
		string sqlCmd = @"select toT.Translation as TheTranslation, toT.LanguageCode,fromWt.CreatedByUserName,fromWt.CreatedTimeStamp 
							from Taggloo_Word fromWord 
							inner join Taggloo_WordTranslation fromWt on fromWt.WordID=fromWord.ID
							inner join Taggloo_Translation toT on fromWt.TranslationID=toT.ID
							where fromWord.Word=@word";
		IEnumerable<WordTranslation> translations = await sqlConnection.QueryAsync<WordTranslation>(sqlCmd, new
		{
			Word = wordInLanguage.TheWord
		});
		
		// make sure all the returned languages are supported
		IEnumerable<WordTranslation> translationsForWord = translations as WordTranslation[] ?? translations.ToArray();
		return translationsForWord;
		
	}
	
	private async Task<IEnumerable<PhraseTranslation>> GetTranslationsForPhrase(SqlConnection sqlConnection,
		Phrase phraseInLanguage)
	{
		string sqlCmd = @"select t.Translation,t.LanguageCode,pt.CreatedByUserName,pt.CreatedTimeStamp from Taggloo_Phrase p
			inner join Taggloo_PhraseTranslation pt on p.id=pt.PhraseID
			inner join Taggloo_Translation t on t.ID=pt.TranslationID
			where p.Phrase=@phrase";

		IEnumerable<PhraseTranslation> translations = await sqlConnection.QueryAsync<PhraseTranslation>(sqlCmd, new
		{
			Phrase = phraseInLanguage.ThePhrase
		});
		
		// make sure all the returned languages are supported
		IEnumerable<PhraseTranslation> translationsForPhrase = translations as PhraseTranslation[] ?? translations.ToArray();
		return translationsForPhrase;
		
	}
	
	
	
	private async Task UpdateWordTranslationAtTargetWithMetaData(HttpClient httpClient, int id, string translationCreatedByUserName, DateTime translationCreatedAt)
	{
		string url = $"/api/v4/translations/word/{id}";
		UpdateWordTranslation updateWordTranslation = new UpdateWordTranslation()
		{
			CreatedByUserName = translationCreatedByUserName,
			CreatedAt = translationCreatedAt
		};
		
		HttpResponseMessage response = await httpClient.PatchAsJsonAsync(url, updateWordTranslation);
		response.EnsureSuccessStatusCode();
	}
	
	
	private async Task UpdatePhraseAtTargetWithMetaData(HttpClient httpClient, int id, string createdByUserName, DateTime createdAtTimeStamp)
	{
		string url = "/api/v4/phrases";
		UpdatePhrase updatePhrase = new UpdatePhrase()
		{
			PhraseId = id,
			CreatedByUserName = createdByUserName,
			CreatedAt = createdAtTimeStamp
		};
		
		HttpResponseMessage response = await httpClient.PatchAsJsonAsync(url, updatePhrase);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}
	}
	

	private async Task<CreatePhraseResult> PostPhraseToTarget(HttpClient httpClient, string phraseInLanguage,
		int dictionaryId)
	{
		string url = "/api/v4/phrases";
		CreatePhrase createPhrase = new CreatePhrase()
		{
			Phrase = phraseInLanguage,
			DictionaryId = dictionaryId
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


	
	
	private async Task<bool> IsLanguageKnown(HttpClient httpClient, string languageCode)
	{
		string url = "/api/v4/languages";
		HttpResponseMessage response = await httpClient.GetAsync(url+$"/{languageCode}");
		if (response.StatusCode == HttpStatusCode.NotFound)
		{
			return false;
		}

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"Error when creating Language {languageCode}");
		}

		return true;
	}

	

	

	private async Task<IEnumerable<string>> GetLanguageCodesFromSource(SqlConnection sqlConnection)
	{
		string sqlCmd = "SELECT Code FROM Taggloo_Language";
		return await sqlConnection.QueryAsync<string>(sqlCmd);
	}


	private async Task<LoginUserResult> ConnectToApi(HttpClient httpClient)
	{
		string url = "/api/v4/login";
		LoginUser loginUser = new LoginUser()
		{
			Password = _importOptions.Password,
			UserName = _importOptions.UserName
		};
		HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, loginUser);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}

		LoginUserResult? loginUserResult = await response.Content.ReadFromJsonAsync<LoginUserResult>();
		return loginUserResult!;
	}

	private SqlConnection ConnectToSqlServer()
	{
		SqlConnection sqlConnection = new SqlConnection(_importOptions.SourceConnectionString);
		try
		{
			sqlConnection.Open();
			return sqlConnection;
		}
		catch (SqlException sqlEx)
		{
			throw new InvalidOperationException($"Unable to connect to SQL Server",sqlEx);
		}
		
	}
}