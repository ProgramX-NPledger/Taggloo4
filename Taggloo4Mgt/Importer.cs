using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Dapper;
using Taggloo4.Dto;
using Taggloo4Mgt.Model;

namespace Taggloo4Mgt;
public class Importer
{
	private readonly ImportOptions _importOptions;

	public Importer(ImportOptions importOptions)
	{
		_importOptions = importOptions;
	}
	
	public async Task<int> Process()
	{
		// connect to SQL Server
		using (SqlConnection sqlConnection = ConnectToSqlServer())
		{
			using (HttpClient httpClient = CreateHttpClient())
			{
				LoginUserResult loginUserResult= await ConnectToApi(httpClient);
				if (loginUserResult == null) throw new InvalidOperationException("Failed to log in to API");
				
				httpClient.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", loginUserResult.Token);
				
				// get all languages
				IEnumerable<string> sourceLanguageCodes = await GetLanguageCodesFromSource(sqlConnection);
			
				// ensure all languages are known at v4
				IEnumerable<string> validLanguageCodes = sourceLanguageCodes as string[] ?? sourceLanguageCodes.ToArray();
				await EnsureAllLanguagesAreKnown(httpClient, validLanguageCodes);

				IDictionary<string, int> dictionariesAtTargetDictionary = new Dictionary<string, int>(); // languageCode, idAtTarget
				foreach (string languageCode in validLanguageCodes)
				{
					// get all words
					IEnumerable<Word> wordsInLanguage = await GetAllWordsForLanguage(sqlConnection,languageCode);

					CreateDictionaryResult createDictionaryResult = await CreateDictionaryForLanguage(httpClient,languageCode);
					dictionariesAtTargetDictionary.Add(languageCode,createDictionaryResult.Id);
					
					// for each Word in source
					foreach (Word wordInLanguage in wordsInLanguage)
					{
						try
						{
							// post word
							CreateWordResult createWordResult=await PostWordToTarget(httpClient, wordInLanguage.TheWord, dictionariesAtTargetDictionary[languageCode]);
							await UpdateWordAtTargetWithMetaData(httpClient, createWordResult.Id, wordInLanguage.CreatedByUserName, wordInLanguage.CreatedTimeStamp);
					
							// get translations
							IEnumerable<Translation> translations = await GetTranslationsForWord(sqlConnection, wordInLanguage, validLanguageCodes);

							foreach (Translation translation in translations)
							{
								if (!dictionariesAtTargetDictionary.ContainsKey(translation.LanguageCode))
								{
									CreateDictionaryResult createOtherDictionaryResult =
										await CreateDictionaryForLanguage(httpClient, translation.LanguageCode);
									dictionariesAtTargetDictionary.Add(translation.LanguageCode,createOtherDictionaryResult.Id);
								}
							
								// post word
								CreateWordResult otherCreateWordResult=await PostWordToTarget(httpClient, translation.TheTranslation, dictionariesAtTargetDictionary[translation.LanguageCode]);
							
								// this will set the creation meta data for the target word to that of the Translation
								await UpdateWordAtTargetWithMetaData(httpClient, otherCreateWordResult.Id, translation.CreatedByUserName,translation.CreatedAt);
							
								// post translation
								CreateWordTranslationResult? createWordTranslationResult = await PostTranslationBetweenWords(httpClient,createWordResult.Id,otherCreateWordResult.Id,dictionariesAtTargetDictionary[languageCode]);

								await UpdateWordTranslationAtTargetWithMetaData(httpClient,
									createWordTranslationResult.Id, translation.CreatedByUserName,
									translation.CreatedAt);
							}

						}
						catch (Exception ex)
						{
							Console.Error.WriteLine($"$Failed to import Word '{wordInLanguage.TheWord}'");
							Exception? exPtr = ex;
							do
							{
								Console.Error.WriteLine(exPtr.Message);
								exPtr = exPtr.InnerException;
							} while (exPtr!=null);
						}
						

					}

				}
				
			}

		}

		return 0;

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
		response.EnsureSuccessStatusCode();

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
		response.EnsureSuccessStatusCode();

		CreateDictionaryResult? createDictionaryResult =
			await response.Content.ReadFromJsonAsync<CreateDictionaryResult>();
		return createDictionaryResult!;
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
		response.EnsureSuccessStatusCode();
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
		response.EnsureSuccessStatusCode();

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
		response.EnsureSuccessStatusCode();

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