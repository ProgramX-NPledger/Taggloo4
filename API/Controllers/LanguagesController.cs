using System.Text;
using System.Text.Encodings.Web;
using API.Contract;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// Language operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LanguagesController : BaseApiController
{
	private readonly ILanguageRepository _languageRepository;


	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="languageRepository">Implementation of <seealso cref="ILanguageRepository"/>.</param>
	public LanguagesController(ILanguageRepository languageRepository)
	{
		_languageRepository = languageRepository;
	}

	/// <summary>
	/// Retrieve matching Languages.
	/// </summary>
	/// <param name="ietfLanguageTag">If specified, the IETF Tag used for the Language.</param>
	/// <returns>A <seealso cref="GetLanguagesResult"/> containing the results.</returns>
	/// <response code="200">Results prepared..</response>
	/// <response code="403">Not permitted.</response>
	[HttpGet("{ietfLanguageTag?}")] //
	[Authorize(Roles="administrator,dataExporter,translator")]
	public async Task<ActionResult<GetLanguagesResult>> GetLanguage(string? ietfLanguageTag)
	{
		AssertApiConstraints(2);

		IEnumerable<Language> languages = (await _languageRepository.GetLanguagesAsync(ietfLanguageTag)).ToArray();

		string queryString = BuildQueryString(ietfLanguageTag);
		GetLanguagesResult getLanguagesResult = new GetLanguagesResult()
		{
			Results = languages.Select(l => new GetLanguageResultItem()
			{
				Name = l.Name,
				IetfLanguageCode = l.IetfLanguageTag,
				Links = new[]
				{
					new Link()
					{
						Action = "get",
						Rel = "self",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/languages/{l.IetfLanguageTag}"
					}
				}
			}),
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new [] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/languages?{queryString}"
				}
			},
			FromIndex = 0,
			PageSize = languages.Count(),
			TotalItemsCount = languages.Count()
		};
		return getLanguagesResult;
	}

	private string BuildQueryString(string? ietfLanguageTag)
	{
		StringBuilder sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(ietfLanguageTag)) sb.Append($"ietfLanguageTag={UrlEncoder.Default.Encode(ietfLanguageTag)}");
		if (sb.Length > 0) sb.Append("&");
		return sb.ToString();
	}


	/// <summary>
	/// Creates a new Language.
	/// </summary>
	/// <param name="createLanguage">A <see cref="CreateLanguage"/> representing the Language to create.</param>
	/// <returns>The created Language.</returns>
	/// <response code="201">Language was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost]
	[Authorize(Roles="administrator")]
	public async Task<ActionResult<AppUser>> CreateLanguage(CreateLanguage createLanguage)
	{
		IEnumerable<Language> allLanguages = await _languageRepository.GetAllLanguagesAsync();
			
		// make sure language is not already defined
		if (allLanguages.Select(q => q.IetfLanguageTag.ToLower())
		    .Any(q => q == createLanguage.IetfLanguageTag.ToLower()))
		{
			return BadRequest("Language already defined with used IETF Language Tag");
		}

		// make sure there will only be a maximum of two created Languges
		if (allLanguages.Count() + 1 > 2)
		{
			return BadRequest("Adding another Language would exceed maximum number of supported Languages");
		}

		string correctlyCasedIetfLanguageTag = EnsureCorrectlyCasedIetfLanguageTag(createLanguage.IetfLanguageTag);
		Language language = new Language()
		{
			IetfLanguageTag = correctlyCasedIetfLanguageTag,
			Name = createLanguage.Name
		};
		
		_languageRepository.Create(language);
		if (!await _languageRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/languages/{correctlyCasedIetfLanguageTag}";
		CreateLanguageResult createLanguageResult = new CreateLanguageResult()
		{
			IetfLanguageTag = correctlyCasedIetfLanguageTag,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new string[] { JSON_MIME_TYPE },
					HRef = url
				}
			}
		};
		return Created(url,createLanguageResult);
		
		// there is no need to instigate wider indexing, because language code is new and no data could
		// have been created with it, yet.
	}

	private string EnsureCorrectlyCasedIetfLanguageTag(string ietfLanguageTag)
	{
		if (ietfLanguageTag.Contains("-"))
		{
			string[] split = ietfLanguageTag.Split(new char[] { '-' });
			split[0] = split[0].ToLower();
			string rejoined = string.Join('-', split);
			return rejoined;
		}

		return ietfLanguageTag;
	}
}