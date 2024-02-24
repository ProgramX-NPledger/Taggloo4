using API.Contract;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// Dictionary operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DictionariesController : BaseApiController
{
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly ILanguageRepository _languageRepository;


	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="dictionaryRepository">Implementation of <seealso cref="IDictionaryRepository"/>.</param>
	/// <param name="languageRepository">Implementation of <seealso cref="ILanguageRepository"/>.</param>
	public DictionariesController(IDictionaryRepository dictionaryRepository,
		ILanguageRepository languageRepository)
	{
		_dictionaryRepository = dictionaryRepository;
		_languageRepository = languageRepository;
	}


	
    /// <summary>
	/// Creates a new Dictionary.
	/// </summary>
	/// <param name="createDictionary">A <see cref="CreateDictionary"/> representing the Dictionary to create.</param>
	/// <returns>The created Language.</returns>
	/// <response code="201">Dictionary was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost]
	[Authorize(Roles="administrator,dataImporter")]
	public async Task<ActionResult<CreateDictionaryResult>> CreateDictionary(CreateDictionary createDictionary)
	{
		Language? language = await _languageRepository.GetLanguageByIetfLanguageTagAsync(createDictionary.IetfLanguageTag);
		if (language == null) return BadRequest("Invalid Language");
		
		Dictionary newDictionary = new Dictionary()
		{
			CreatedAt = DateTime.Now,
			CreatedOn = GetRemoteHostAddress(),
			CreatedByUserName = GetCurrentUserName(),
			Name = createDictionary.Name,
			IetfLanguageTag = language.IetfLanguageTag,
			Description = createDictionary.Description,
			SourceUrl = createDictionary.SourceUrl
		};

		_dictionaryRepository.Create(newDictionary);
		if (!await _dictionaryRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/dictionaries/{newDictionary.Id}";
		CreateDictionaryResult createDictionaryResult = new CreateDictionaryResult()
		{
			Id = newDictionary.Id,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new[] { JSON_MIME_TYPE },
					HRef = url
				},
				new Link()
				{
					Action = "get",
					Rel = "language",
					Types = new[] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/languages/{newDictionary.IetfLanguageTag}" 
				}
			}
		};
		
		return Created(url,createDictionaryResult);
	}

}