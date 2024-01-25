using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.Helper;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LanguagesController : BaseApiController
{
	private readonly ILanguageRepository _languageRepository;


	public LanguagesController(ILanguageRepository languageRepository)
	{
		_languageRepository = languageRepository;
	}

// 	/// <summary>
// 	/// Gets all Users.
// 	/// </summary>
// 	/// <returns></returns>
// 	/// <response code="200">Request was successful.</response>
// 	[HttpGet]
// 	// TODO: Add parameters to allow filtering, paging, return 400 if bad request
// 	public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
// 	{
// 		return Ok(await _userManager.Users.ToListAsync());
// //		return users; // TODO use RESTful DTO
// 	}
//
	

	/// <summary>
	/// Retrieve Language details.
	/// </summary>
	/// <param name="ietfLanguageTag">The IETF Tag used for the Language.</param>
	/// <returns>A user.</returns>
	/// <response code="200">Language is found.</response>
	/// <response code="403">Not permitted.</response>
	/// <response code="404">Language is not found.</response>
	[HttpGet("{ietfLanguageTag}")]
	public async Task<ActionResult<GetLanguageResult>> GetLanguage(string ietfLanguageTag)
	{
		Language? language = await _languageRepository.GetLanguageByIetfLanguageTag(ietfLanguageTag);
		if (language == null) return NotFound();
		
		return new GetLanguageResult()
		{
			IetfLanguageCode = language.IetfLanguageTag,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new[] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/languages/{language.IetfLanguageTag}"
				}
			},
			Name = language.Name
		};
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