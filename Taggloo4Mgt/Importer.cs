using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using Dapper;
using Taggloo4.Dto;
using Taggloo4Mgt.Model;

namespace Taggloo4Mgt;
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
				LoginUserResult loginUserResult= await ConnectToApi(httpClient);
				if (loginUserResult == null) throw new InvalidOperationException("Failed to log in to API");
				Log("\t\tOk");
				
				httpClient.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", loginUserResult.Token);
				
				// get all languages
				Log("\t\tGet all Languages");
				IEnumerable<string> sourceLanguageCodes = await GetLanguageCodesFromSource(sqlConnection);
				Log($"\t\t\t{sourceLanguageCodes.Count()} Languages");

				// have to create both languages before we start
				foreach (string languageCode in sourceLanguageCodes)
				{
					if (!await IsLanguageKnown(httpClient,languageCode))
					{
						CreateLanguageResult createLanguageResult=await CreateLanguage(httpClient,languageCode);
						Log($"\t\t\tCreated Language for {languageCode}");
					}
				}

				if (_importOptions.ResetAllDictionaries)
				{
					// reset all existing dictionaries for languages prior to import
					await DeleteAllDictionariesForLanguage(httpClient, sourceLanguageCodes);
				}
				
				IDictionary<string, int> dictionariesAtTargetDictionary = new Dictionary<string, int>(); // languageCode, idAtTarget
				int wordsProcessed = 0;
				int totalWords = 0;

				foreach (string languageCode in sourceLanguageCodes)
				{
				
					
					UpdateProgressBar(0, 0, "", "");

					Log($"\t\t\t{languageCode}");
						
					// get all words
					IEnumerable<Word> wordsInLanguage = await GetAllWordsForLanguage(sqlConnection,languageCode);
					Log($"\t\t\t\t{wordsInLanguage.Count()} words in Language");
					totalWords += wordsInLanguage.Count();

					// make sure dictionary hasn't already been created
					if (!dictionariesAtTargetDictionary.ContainsKey(languageCode))
					{
						Log("\t\t\t\tCreate Dictionary");
						CreateDictionaryResult createDictionaryResult = await CreateDictionaryForLanguage(httpClient,languageCode);
						Log($"\t\t\t\tDictionary ID {createDictionaryResult.Id} created");
						dictionariesAtTargetDictionary.Add(languageCode,createDictionaryResult.Id);
					}
					else
					{
						Log($"\t\t\t\tDictionary for Language {languageCode} already created, has ID {dictionariesAtTargetDictionary[languageCode]}");
					}
					
					// for each Word in source
					foreach (Word wordInLanguage in wordsInLanguage)
					{
						DateTime startTimeStamp = DateTime.Now;
						
						Log($"\t\t\t\t\t{wordInLanguage.TheWord}");

						if (_importOptions.MaxWordsPerLanguage.HasValue &&
						    _importOptions.MaxWordsPerLanguage.Value < wordsProcessed)
						{
							// already processed enough words, so ignore
						}
						else
						{
							try
							{
								ProcessWord(httpClient, sqlConnection, wordInLanguage, dictionariesAtTargetDictionary,
									languageCode);
							
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
							
						}

						TimeSpan delta = DateTime.Now - startTimeStamp;
						_millisecondsBetweenWords.Add((int)delta.TotalMilliseconds);
						
						
						UpdateProgressBar(wordsProcessed++, totalWords, languageCode, wordInLanguage.TheWord);
						
						
					}

						
				}
				
				
				
			}

		}

		Console.WriteLine();
		
		return 0;

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

	private async Task ProcessWord(HttpClient httpClient, SqlConnection sqlConnection, Word wordInLanguage, IDictionary<string,int> dictionariesAtTargetDictionary, string languageCode)
	{
		
		// does the word already exist?
		GetWordsResult existingWord = await GetWord(httpClient, wordInLanguage.TheWord,
			dictionariesAtTargetDictionary[languageCode]);
		if (existingWord.Results.Any())
		{
			// word already exists, so have to assume translations do too
			Log($"\t\t\t\t\t\tWord ID already exists");
		}
		else
		{
			// post word
			CreateWordResult createWordResult=await PostWordToTarget(httpClient, wordInLanguage.TheWord, dictionariesAtTargetDictionary[languageCode]);
			await UpdateWordAtTargetWithMetaData(httpClient, createWordResult.Id, wordInLanguage.CreatedByUserName, wordInLanguage.CreatedTimeStamp);
			Log($"\t\t\t\t\t\tWord ID {createWordResult.Id} created");
			
			// get translations
			IEnumerable<Translation> translations = await GetTranslationsForWord(sqlConnection, wordInLanguage);
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
				GetWordsResult getOtherWordResult = await GetWord(httpClient,
					translation.TheTranslation, dictionariesAtTargetDictionary[translation.LanguageCode]);
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
	

	private async Task<GetWordsResult> GetWord(HttpClient httpClient, string word, int dictionaryId)
	{
		string url = $"/api/v4/words?word={word}&dictionaryId={dictionaryId}";

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
		Word wordInLanguage)
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
		return translationsForWord;
		
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