using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Dapper;
using Taggloo4.Dto;

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
				EnsureAllLanguagesAreKnown(httpClient, sourceLanguageCodes);

				foreach (string languageCode in sourceLanguageCodes)
				{
					// get all words
					IEnumerable<string> wordsInLanguage = GetAllWordsForLanguage(languageCode);

					// for each Word in source
					foreach (string wordInLanguage in wordsInLanguage)
					{
						// post word
						PostWordToTarget(wordInLanguage);
					
						// get translations
						object translations = GetTranslationsForWord(wordInLanguage);

						foreach (object translation in translations)
						{
							// for each translation, get word
							string otherWord = (string)translation;
							
							// post word
							PostWordToTarget(otherWord,otherLanguage);
						
							// post translation
							PostTranslationBetweenWords(,);

						}

					}

				}
				
			}

		}

		return 0;

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
		string sqlCmd = "SELECT LanguageCode FROM Taggloo_Language";
		return await sqlConnection.QueryAsync<string>(sqlCmd);
	}

	private HttpClient CreateHttpClient()
	{
		HttpClient httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Accept.Clear();
		httpClient.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));
		httpClient.DefaultRequestHeaders.Add("user-Agent", "Taggloo4Mgt utility");
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
		SqlConnection sqlConnection = new SqlConnection(_importOptions.SourceConnectionstring);
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