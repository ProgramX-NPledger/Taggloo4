﻿using System.Data.SqlClient;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dapper;
using Taggloo4.Dto;
using Taggloo4Mgt.Model;

namespace Taggloo4Mgt;
public class Importer
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
			using (HttpClient httpClient = CreateHttpClient())
			{
				Log("\tOk");
				
				Log($"\tLog in to API as {_importOptions.UserName}");
				LoginUserResult loginUserResult= await ConnectToApi(httpClient);
				if (loginUserResult == null) throw new InvalidOperationException("Failed to log in to API");
				Log("\t\tOk");
				
				httpClient.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", loginUserResult.Token);
				
				// get all languages
				Log("\t\tGet all Languages");
				IEnumerable<string> sourceLanguageCodes = await GetLanguageCodesFromSource(sqlConnection);
				Log($"\t\t\t{sourceLanguageCodes.Count()} Languages");
			
				// ensure all languages are known at v4
				IEnumerable<string> validLanguageCodes = sourceLanguageCodes as string[] ?? sourceLanguageCodes.ToArray();
				await EnsureAllLanguagesAreKnown(httpClient, validLanguageCodes);
				Log("\t\t\tAll Languages are valid");
				
				IDictionary<string, int> dictionariesAtTargetDictionary = new Dictionary<string, int>(); // languageCode, idAtTarget
				int wordsProcessed = 0;
				int totalWords = 0;

				UpdateProgressBar(0, 0, "", "");
				foreach (string languageCode in validLanguageCodes)
				{
					Log($"\t\t\t{languageCode}");
					
					// get all words
					IEnumerable<Word> wordsInLanguage = await GetAllWordsForLanguage(sqlConnection,languageCode);
					Log($"\t\t\t\t{wordsInLanguage.Count()} words in Language");
					totalWords += wordsInLanguage.Count();

					Log("\t\t\t\tCreate Dictionary");
					CreateDictionaryResult createDictionaryResult = await CreateDictionaryForLanguage(httpClient,languageCode);
					Log($"\t\t\t\tDictionary ID {createDictionaryResult.Id} created");
					dictionariesAtTargetDictionary.Add(languageCode,createDictionaryResult.Id);
					
					// for each Word in source
					foreach (Word wordInLanguage in wordsInLanguage)
					{
						DateTime startTimeStamp = DateTime.Now;
						
						Log($"\t\t\t\t\t{wordInLanguage.TheWord}");
						try
						{
							// post word
							CreateWordResult createWordResult=await PostWordToTarget(httpClient, wordInLanguage.TheWord, dictionariesAtTargetDictionary[languageCode]);
							await UpdateWordAtTargetWithMetaData(httpClient, createWordResult.Id, wordInLanguage.CreatedByUserName, wordInLanguage.CreatedTimeStamp);
							Log($"\t\t\t\t\t\tWord ID {createWordResult.Id} created");
							
							// get translations
							IEnumerable<Translation> translations = await GetTranslationsForWord(sqlConnection, wordInLanguage, validLanguageCodes);
							Log($"\t\t\t\t\t\t{translations.Count()} Translations");
							
							foreach (Translation translation in translations)
							{
								Log($"\t\t\t\t\t\t\t{translation.TheTranslation} ({translation.LanguageCode})");
								if (!dictionariesAtTargetDictionary.ContainsKey(translation.LanguageCode))
								{
									CreateDictionaryResult createOtherDictionaryResult =
										await CreateDictionaryForLanguage(httpClient, translation.LanguageCode);
									Log($"\t\t\t\t\t\t\tDictionary ID {createOtherDictionaryResult.Id} created for {translation.LanguageCode}");
									dictionariesAtTargetDictionary.Add(translation.LanguageCode,createOtherDictionaryResult.Id);
								}
							
								// post word
								// we need to see if the word already exists. If it does, do not create another word, instead link the existing
								GetWordsResult getOtherWordResult = await GetOtherWord(httpClient,
									translation.TheTranslation, dictionariesAtTargetDictionary[languageCode]);
								int otherWordId;
								if (getOtherWordResult.TotalItemsCount == 0)
								{
									// the word doesn't already exist
									CreateWordResult otherCreateWordResult = await PostWordToTarget(httpClient,
										translation.TheTranslation,
										dictionariesAtTargetDictionary[translation.LanguageCode]);
									otherWordId = otherCreateWordResult.Id;

									// this will set the creation meta data for the target word to that of the Translation
									await UpdateWordAtTargetWithMetaData(httpClient, otherWordId, translation.CreatedByUserName,translation.CreatedAt);
									Log($"\t\t\t\t\t\t\tWord ID {otherWordId} created");

								}
								else
								{
									otherWordId = getOtherWordResult.Results.First().Id;
									Log($"\t\t\t\t\t\t\tWord ID {otherWordId} already exists, adding translation");
								}
								
								// post translation
								CreateWordTranslationResult? createWordTranslationResult = await PostTranslationBetweenWords(httpClient,createWordResult.Id,otherWordId,dictionariesAtTargetDictionary[languageCode]);

								await UpdateWordTranslationAtTargetWithMetaData(httpClient,
									createWordTranslationResult!.Id, translation.CreatedByUserName,
									translation.CreatedAt);
								Log($"\t\t\t\t\t\t\tTranslation ID {createWordTranslationResult.Id} created");
							}

							UpdateProgressBar(wordsProcessed, totalWords, languageCode, wordInLanguage.TheWord);
						}
						catch (Exception ex)
						{
							await Console.Error.WriteLineAsync();
							await Console.Error.WriteLineAsync($"Failed to import Word '{wordInLanguage.TheWord}'");
							Exception? exPtr = ex;
							do
							{
								await Console.Error.WriteLineAsync(exPtr.Message);
								Log($"ERROR: {exPtr.GetType().Name}: {exPtr.Message}");
								exPtr = exPtr.InnerException;
							} while (exPtr!=null);
						}

						TimeSpan delta = DateTime.Now - startTimeStamp;
						_millisecondsBetweenWords.Add((int)delta.TotalMilliseconds);
						
						wordsProcessed++;
						
					}

				}
				
			}

		}

		Console.WriteLine();
		
		return 0;

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
			File.AppendAllLines(_logFileName!,new string[]
			{
				s
			});
		}
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

	private async Task<CreateDictionaryResult> CreateDictionaryForLanguage(HttpClient httpClient, string languageCode)
	{
		string url = "/api/v4/dictionaries";
		CreateDictionary createDictionary = new CreateDictionary()
		{
			Name = $"Imported from Taggloo2",
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

	private async Task<GetWordsResult> GetOtherWord(HttpClient httpClient, string word, int dictionaryId)
	{
		string url = $"/api/v4/words/{word}?dictionaryId={dictionaryId}";

		HttpResponseMessage response = await httpClient.GetAsync(url);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(
				$"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}

		GetWordsResult? getWordsResult = await response.Content.ReadFromJsonAsync<GetWordsResult>();
		return getWordsResult!;

	}

	
	private async Task<IEnumerable<Translation>> GetTranslationsForWord(SqlConnection sqlConnection,
		Word wordInLanguage, IEnumerable<string> sourceLanguageCodes)
	{
		string sqlCmd = @"select toT.Translation as TheTranslation, toT.LanguageCode,fromWt.CreatedByUserName,fromWt.CreatedTimeStamp 
							from Taggloo_Word fromWord 
							inner join Taggloo_WordTranslation fromWt on fromWt.WordID=fromWord.ID
							inner join Taggloo_Translation toT on fromWt.TranslationID=toT.ID
							where fromWord.Word=@word";
		IEnumerable<Translation> translations = await sqlConnection.QueryAsync<Translation>(sqlCmd, new
		{
			Word = wordInLanguage.TheWord
		});
		
		// make sure all the returned languages are supported
		IEnumerable<Translation> translationsForWord = translations as Translation[] ?? translations.ToArray();
		if (translationsForWord.All(q => sourceLanguageCodes.Contains(q.LanguageCode))) return translationsForWord;
		throw new InvalidOperationException($"Unsupported translatable Language detected");
	}

	private async Task UpdateWordAtTargetWithMetaData(HttpClient httpClient, int id, string createdByUserName, DateTime createdAtTimeStamp)
	{
		string url = "/api/v4/words";
		UpdateWord updateWord = new UpdateWord()
		{
			WordId = id,
			CreatedByUserName = createdByUserName,
			CreatedAt = createdAtTimeStamp
		};
		
		HttpResponseMessage response = await httpClient.PatchAsJsonAsync(url, updateWord);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}
	}

	private async Task<CreateWordResult> PostWordToTarget(HttpClient httpClient, string wordInLanguage,
		int dictionaryId)
	{
		string url = "/api/v4/words";
		CreateWord createWord = new CreateWord()
		{
			Word = wordInLanguage,
			DictionaryId = dictionaryId
		};
		
		HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createWord);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
		}

		CreateWordResult? createWordResult=
			await response.Content.ReadFromJsonAsync<CreateWordResult>();
		return createWordResult!;
	}


	private async Task<IEnumerable<Word>> GetAllWordsForLanguage(SqlConnection sqlConnection, string languageCode)
	{
		string sqlCmd =
			"SELECT ID,Word as TheWord, LanguageCode, CreatedTimeStamp, CreatedByUserName, IsBlocked, BlockedByUserName, BlockedTimeStamp FROM Taggloo_Word WHERE LanguageCode=@LanguageCode";

		return await sqlConnection.QueryAsync<Word>(sqlCmd, new
		{
			LanguageCode = languageCode
		});
	}

	private async Task EnsureAllLanguagesAreKnown(HttpClient httpClient, IEnumerable<string> sourceLanguageCodes)
	{
		foreach (string languageCode in sourceLanguageCodes)
		{
			string url = "/api/v4/languages";
			HttpResponseMessage response = await httpClient.GetAsync(url+$"/{languageCode}");
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException(
					$"Invalid IETF Language Tag {languageCode}. Target does not support Language from Source");
			}
		}
	}

	private async Task<IEnumerable<string>> GetLanguageCodesFromSource(SqlConnection sqlConnection)
	{
		string sqlCmd = "SELECT Code FROM Taggloo_Language";
		return await sqlConnection.QueryAsync<string>(sqlCmd);
	}

	private HttpClient CreateHttpClient()
	{
		HttpClientHandler httpClientHandler = new HttpClientHandler();
		#if DEBUG
		httpClientHandler.ServerCertificateCustomValidationCallback =
			HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
		#endif
		
		HttpClient httpClient = new HttpClient(httpClientHandler);
		// Server rejects when added this header
		// httpClient.DefaultRequestHeaders.Accept.Clear();
		// httpClient.DefaultRequestHeaders.Accept.Add(
		// 	new MediaTypeWithQualityHeaderValue("application/json"));
		httpClient.DefaultRequestHeaders.Add("User-Agent", "Taggloo4Mgt utility");
		httpClient.BaseAddress = new Uri(_importOptions.Url);
		
		return httpClient;
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
			throw new InvalidOperationException($"Unable to connect to SQL Server");
		}
		
	}
}