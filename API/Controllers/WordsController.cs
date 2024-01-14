using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.DTO;
using API.Helper;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WordsController : BaseApiController
{
	private readonly IWordRepository _wordRepository;
	private readonly IDictionaryRepository _dictionaryRepository;


	public WordsController(IWordRepository wordRepository, IDictionaryRepository dictionaryRepository)
	{
		_wordRepository = wordRepository;
		_dictionaryRepository = dictionaryRepository;
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
	/// Creates a new Word.
	/// </summary>
	/// <param name="createWord">A <see cref="CreateWord"/> representing the Word to create.</param>
	/// <returns>The created Word.</returns>
	/// <response code="201">Word was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost]
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> CreateWord(CreateWord createWord)
	{
		// try to resolve the dictionary
		Dictionary? dictionary = await _dictionaryRepository.GetById(createWord.DictionaryId);
		if (dictionary == null) return BadRequest("Invalid Dictionary");
		
		// does the word for the language already exist? If so, reject - maybe a translation is required
		Word? existingWord = await _wordRepository.GetWordByWord(createWord.Word);
		if (existingWord != null)
			return BadRequest("Word already exists, perhaps a Translation from the existing Word is appropriate?");
		
		Word newWord = new Word()
		{
			CreatedAt = DateTime.Now,
			CreatedOn = GetRemoteHostAddress(),
			CreatedByUserName = GetCurrentUserName(),
			TheWord = createWord.Word,
			DictionaryId = dictionary.Id
		};

		_wordRepository.Create(newWord);
		if (!await _wordRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/words/{newWord.Id}";
		CreateWordResult createWordResult = new CreateWordResult()
		{
			Id = newWord.Id,
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
					HRef = $"{GetBaseApiPath()}/dictionaries/{newWord.DictionaryId}"
				}
			}
		};
		
		// TODO: Scan phrases/etc. for instances of Word within this language and link
		
		return Created(url,createWordResult);
		
		
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