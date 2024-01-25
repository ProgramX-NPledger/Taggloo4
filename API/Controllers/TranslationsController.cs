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
public class TranslationsController : BaseApiController
{
	private readonly IWordRepository _wordRepository;
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly ITranslationRepository _translationRepository;


	public TranslationsController(IWordRepository wordRepository, 
		IDictionaryRepository dictionaryRepository,
		ITranslationRepository translationRepository)
	{
		_wordRepository = wordRepository;
		_dictionaryRepository = dictionaryRepository;
		_translationRepository = translationRepository;
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
	/// Creates a new Word Translation.
	/// </summary>
	/// <param name="createWordTranslation">A <see cref="CreateWordTranslation"/> representing the Translation to create.</param>
	/// <returns>The created Translation.</returns>
	/// <response code="201">Word was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost("word")] // /api/v4/translations/word
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> CreateTranslation(CreateWordTranslation createWordTranslation)
	{
		// try to resolve the dictionary
		Dictionary? dictionary = await _dictionaryRepository.GetById(createWordTranslation.DictionaryId);
		if (dictionary == null) return BadRequest("Invalid Dictionary");
		
		// do the words exist?
		Word? fromWord = await _wordRepository.GetById(createWordTranslation.FromWordId);
		if (fromWord == null) return BadRequest("From Word does not exist");
		Word? toWord = await _wordRepository.GetById(createWordTranslation.ToWordId);
		if (toWord == null) return BadRequest("To Word does not exist");

		WordTranslation wordTranslation=new WordTranslation()
		{
			CreatedAt = DateTime.Now,
			CreatedOn = GetRemoteHostAddress(),
			CreatedByUserName = GetCurrentUserName(),
			DictionaryId = createWordTranslation.DictionaryId,
			FromWordId = createWordTranslation.FromWordId,
			ToWordId = createWordTranslation.ToWordId
		};

		_translationRepository.Create(wordTranslation);
		if (!await _wordRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/translations/{wordTranslation.Id}";
		CreateWordTranslationResult createWordTranslationResult = new CreateWordTranslationResult()
		{
			Id = wordTranslation.Id,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new string[] { JSON_MIME_TYPE },
					HRef = url
				},
				new Link()
				{
					Action = "get",
					Rel = "dictionary",
					Types = new [] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/dictionaries/{wordTranslation.DictionaryId}"
				},
				new Link()
				{
					Action="get",
					Rel="fromWord",
					Types = new [] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/words/{wordTranslation.FromWordId}"
				},
				new Link()
				{
					Action="get",
					Rel="toWord",
					Types=new [] { JSON_MIME_TYPE },
					HRef=$"{GetBaseApiPath()}/words/{wordTranslation.ToWordId}"
				}
			}
		};
		
		return Created(url,createWordTranslationResult);
		
		
	}
    
	/// <summary>
	/// Update an existing Word with meta-data.
	/// </summary>
	/// <param name="updateWord">A <see cref="UpdateWord"/> representing the Word to update.</param>
	/// <returns>The updated Word.</returns>
	/// <response code="201">Word was updated.</response>
	/// <response code="400">One or more validation errors prevented successful updating.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPatch("word/{wordTranslationId}")] // /api/v4/translations/word
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> UpdateWordTranslation(int wordTranslationId, UpdateWordTranslation updateWordTranslation)
	{
		// does the translation exist?
		WordTranslation? wordTranslation = await _translationRepository.GetById(wordTranslationId);
		if (wordTranslation == null) return NotFound();
		

		
		// update word
		UpdateWordTranslationResult updateWordTranslationResult = new UpdateWordTranslationResult()
		{

		};

		if (updateWordTranslation.FromWordId.HasValue) wordTranslation.FromWordId = updateWordTranslation.FromWordId.Value;
		if (updateWordTranslation.ToWordId.HasValue) wordTranslation.ToWordId = updateWordTranslation.ToWordId.Value;
		if (updateWordTranslation.DictionaryId.HasValue)
		{
			// does the new dictionary exist?
			Dictionary? newDictionary = await _dictionaryRepository.GetById(updateWordTranslation.DictionaryId.Value);
			if (newDictionary == null) return BadRequest("Invalid attempt to move Word into non-existent Dictionary");
			
			// is the new dictionary the same language as the previous?
			Dictionary? oldDictionary = await _dictionaryRepository.GetById(wordTranslation.DictionaryId);

			if (!newDictionary.IetfLanguageTag.Equals(oldDictionary!.IetfLanguageTag))
			{
				return BadRequest("Invalid attempt to move Word into Dictionary of another Language");
			}

			wordTranslation.DictionaryId = updateWordTranslation.DictionaryId.Value;
			updateWordTranslationResult.RequiresDictionaryReindexing = true;
		}

		if (updateWordTranslation.CreatedOn != null) wordTranslation.CreatedOn = updateWordTranslation.CreatedOn;
		if (updateWordTranslation.CreatedByUserName != null) wordTranslation.CreatedByUserName = updateWordTranslation.CreatedByUserName;
		if (updateWordTranslation.CreatedAt.HasValue) wordTranslation.CreatedAt = updateWordTranslation.CreatedAt.Value;
		
		_translationRepository.Update(wordTranslation);
		if (!await _translationRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/translations/word/{wordTranslation.Id}";
		updateWordTranslationResult.Id = wordTranslation.Id;
		updateWordTranslationResult.Links = new[]
		{
			new Link()
			{
				Action = "get",
				Rel = "self",
				Types = new string[] { JSON_MIME_TYPE },
				HRef = url
			},
			new Link()
			{
				Action = "get",
				Rel = "dictionary",
				Types = new[] { JSON_MIME_TYPE },
				HRef = $"{GetBaseApiPath()}/dictionaries/{wordTranslation.DictionaryId}"
			},
			new Link()
			{
				Action="get",
				Rel="fromWord",
				Types = new [] { JSON_MIME_TYPE },
				HRef = $"{GetBaseApiPath()}/words/{wordTranslation.FromWordId}"
			},
			new Link()
			{
				Action="get",
				Rel="toWord",
				Types=new [] { JSON_MIME_TYPE },
				HRef=$"{GetBaseApiPath()}/words/{wordTranslation.ToWordId}"
			}
		};
		
		// TODO: Scan phrases/etc. for instances of Word within this language and link
		
		return Ok(updateWordTranslationResult);
		
		
	}

	

	
}