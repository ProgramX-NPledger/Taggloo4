﻿using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Loader;
using System.Text;
using Dapper;
using Taggloo4.Dto;
using Taggloo4.ImportFromTaggloo2.Helpers;
using Taggloo4Mgt.Importing.Importers;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing;
public class Importer : ApiClientBase
{
	private readonly ImportOptions _importOptions;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly string? _logFileName;
	private readonly IList<double> _millisecondsBetweenWords = new List<double>();

	private Dictionary<string, Dictionary<int, string>> _originalIdsToImportIdsMap =
		new Dictionary<string, Dictionary<int, string>>();
	
	public Importer(ImportOptions importOptions, IHttpClientFactory httpClientFactory)
	{
		_importOptions = importOptions;
		_httpClientFactory = httpClientFactory;
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
			Log($"Connect to Taggloo4.Web at {_importOptions.Url}");
			HttpClient httpClient = CreateHttpClient(_httpClientFactory, _importOptions.Url);
		
			Log("\tOk");

			Log($"\tLog in to Taggloo4.Web as {_importOptions.UserName}");
			LoginUserResult loginUserResult = await ConnectToApi(httpClient);
			if (loginUserResult == null) throw new InvalidOperationException("Failed to log in to Taggloo4.Web");
			Log("\t\tOk");

			httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", loginUserResult.Token);

			IEnumerable<string> sourceLanguageCodes =
				await GetLanguagesFromSourceAndEnforceAtTarget(sqlConnection, httpClient);
			
			IEnumerable<IImporter> importers = ImporterFactory.GetImporters();

			// if no import types specified, select all
			if (_importOptions.ImportTypes==null || !_importOptions.ImportTypes.Any())
				_importOptions.ImportTypes = importers.Select(q => q.Key);
			Log($"\t\tImporting: {string.Join(", ", _importOptions.ImportTypes.ToArray())}");

			// prepare all imports
			List<IImportSession> importSessions = new List<IImportSession>();
			int toBeImportedCount = 0;
			int totalImportedCount = 0;
			
			foreach (IImporter importer in importers)
			{
				if (_importOptions.ImportTypes.Select(q=>q.ToLower()).Contains(importer.Key.ToLower()))
				{
					IImportSession importSession = await importer.CreateSession(sqlConnection);
					toBeImportedCount += importSession.GetToBeImportedCount();
					importSession.UpdateMetrics += (sender, e) =>
					{
						_millisecondsBetweenWords.Add(e.MillisecondsBetweenImports);
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
						if (e.IsSuccess)
						{
							if (!_originalIdsToImportIdsMap.ContainsKey(nameof(sender)))
							{
								_originalIdsToImportIdsMap.Add(nameof(sender),new Dictionary<int, string>());
							}

							if (!string.IsNullOrWhiteSpace(e.ExternalId))
							{
								_originalIdsToImportIdsMap[nameof(sender)].Add(e.SourceId,e.ExternalId);	
							}
							
						}
						else
						{
							Console.WriteLine("STOP");
						}
					
						UpdateProgressBar(++totalImportedCount, toBeImportedCount, e.LanguageCode!, e.CurrentItem!, importer.Key);
					};
					importSessions.Add(importSession);
				}
			}

			if (sourceLanguageCodes.Count() != 2)
			{
				throw new InvalidOperationException($"There must be exactly 2 languages defined");
			}
			
			foreach (IImportSession importSession in importSessions)
			{
				// create a Dictionary for each language code
				CreateDictionaryResult dictionary1 = await ImporterHelper.CreateDictionaryForLanguage(httpClient,importSession.ContentTypeKey,
					sourceLanguageCodes.ElementAt(0), importSession);
				CreateDictionaryResult dictionary2 = await ImporterHelper.CreateDictionaryForLanguage(httpClient,importSession.ContentTypeKey,
					sourceLanguageCodes.ElementAt(1), importSession);
				await importSession.ImportAcrossDictionariesAsync(httpClient, sourceLanguageCodes.ElementAt(0), dictionary1.Id,
					sourceLanguageCodes.ElementAt(1), dictionary2.Id);
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


	private void UpdateProgressBar(int wordsProcessed, int totalWords, string languageCode, string theWord, string importType)
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

		string limitedWord = theWord;
		if (limitedWord.Length > 30) limitedWord = limitedWord.Substring(0, 26) + " ...";

		string limitedImportType = importType;
		if (limitedImportType.Length > 15) limitedImportType = limitedImportType.Substring(0, 15) + " ...";
		
		string s = $" {((int)percent).ToString(CultureInfo.InvariantCulture),3}% ({wordsProcessed}/{totalWords}) {languageCode} {limitedWord}";
		Console.Write($"{s,-60}ETA: {eta:hh\\:mm\\:ss} (~ {DateTime.Now.Add(eta):HH\\:mm\\:ss}) {limitedImportType}");
		

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