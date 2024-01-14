using API.Contract;
using API.DTO;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Dictionary operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DictionariesController : BaseApiController
{
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly ILanguageRepository _languageRepository;


	public DictionariesController(IDictionaryRepository dictionaryRepository,
		ILanguageRepository languageRepository)
	{
		_dictionaryRepository = dictionaryRepository;
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
	

	// /// <summary>
	// /// Retrieve user details.
	// /// </summary>
	// /// <param name="userName">User Name of user.</param>
	// /// <returns>A user.</returns>
	// /// <response code="200">User is found.</response>
	// /// <response code="403">Not permitted.</response>
	// /// <response code="404">User is not found.</response>
	// [Authorize(Roles="administrator")]
	// [HttpGet("{userName}")]
	// public async Task<ActionResult<GetUserResult>> GetUser(string userName)
	// {
	// 	string upperedUserName = userName.ToUpper();
	// 	AppUser? user = await _userManager.Users.SingleOrDefaultAsync(q => q.NormalizedUserName == upperedUserName);
	// 	if (user == null) return NotFound();
	//
	// 	List<Link> links = new List<Link>
	// 	{
	// 		new Link()
	// 		{
	// 			Action = "get",
	// 			Rel = "self",
	// 			Types = new string[] { JSON_MIME_TYPE },
	// 			HRef = $"{GetBaseApiPath()}/users/{user.UserName}" 
	// 		}
	// 	};
	//
	// 	IList<string> roles = await _userManager.GetRolesAsync(user);
	// 	roles.ToList().ForEach(x =>
	// 	{
	// 		links.Add(new Link()
	// 		{
	// 			Action = "get",
	// 			Rel = "role",
	// 			Types = new string[] { JSON_MIME_TYPE },
	// 			HRef = $"{GetBaseApiPath()}/roles/{x}"
	// 		});
	// 	});
	// 	
	// 	return new GetUserResult()
	// 	{
	// 		UserName = user.UserName ?? string.Empty,
	// 		HasRoles = await _userManager.GetRolesAsync(user),
	// 		Links = links
	// 	};
	// }
	//
	
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
		Language? language = await _languageRepository.GetLanguageByIetfLanguageTag(createDictionary.IetfLanguageTag);
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