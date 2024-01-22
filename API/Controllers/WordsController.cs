﻿using System.Security.Cryptography;
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
public class WordsController : BaseApiController
{
	private readonly IWordRepository _wordRepository;
	private readonly IDictionaryRepository _dictionaryRepository;


	public WordsController(IWordRepository wordRepository, 
		IDictionaryRepository dictionaryRepository)
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
		Word? existingWord = await _wordRepository.GetWordByWordWithinDictionary(createWord.Word, dictionary.Id);
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
    
	/// <summary>
	/// Update an existing Word with meta-data.
	/// </summary>
	/// <param name="updateWord">A <see cref="UpdateWord"/> representing the Word to update.</param>
	/// <returns>The updated Word.</returns>
	/// <response code="201">Word was updated.</response>
	/// <response code="400">One or more validation errors prevented successful updating.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPatch]
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> UpdateWord(UpdateWord updateWord)
	{
		// does the word for the language already exist? If so, reject - maybe a translation is required
		Word? word = await _wordRepository.GetById(updateWord.WordId);
		if (word == null) return NotFound();
		
		// update word
		UpdateWordResult updateWordResult = new UpdateWordResult()
		{

		};

		if (updateWord.Word!=null &&
		    !word.TheWord.Equals(updateWord.Word))
		{
			// changing the word is dangerous. Ensure the new word doesn't already exist
			Word? newWord = await _wordRepository.GetWordByWordWithinDictionary(updateWord.Word,updateWord.DictionaryId);
			if (newWord != null) return BadRequest("The word is being renamed to another Word that already exists within the Dictionary");

			word.TheWord = updateWord.Word;
		}

		if (updateWord.MoveWordToDictionaryId.HasValue &&
		    word.DictionaryId != updateWord.MoveWordToDictionaryId.Value)
		{
			// does the new dictionary exist?
			Dictionary? newDictionary = await _dictionaryRepository.GetById(updateWord.MoveWordToDictionaryId.Value);
			if (newDictionary == null) return BadRequest("Invalid attempt to move Word into non-existent Dictionary");
			
			// is the new dictionary the same language as the previous?
			Dictionary? oldDictionary = await _dictionaryRepository.GetById(word.DictionaryId);

			if (!newDictionary.IetfLanguageTag.Equals(oldDictionary!.IetfLanguageTag))
			{
				return BadRequest("Invalid attempt to move Word into Dictionary of another Language");
			}

			word.DictionaryId = updateWord.MoveWordToDictionaryId.Value;
			updateWordResult.RequiresDictionaryReindexing = true;
		}

		if (updateWord.CreatedOn != null) word.CreatedOn = updateWord.CreatedOn;
		if (updateWord.CreatedByUserName != null) word.CreatedByUserName = updateWord.CreatedByUserName;
		if (updateWord.CreatedAt.HasValue) word.CreatedAt = updateWord.CreatedAt.Value;
		
		_wordRepository.Update(word);
		if (!await _wordRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/words/{word.Id}";
		updateWordResult.Id = word.Id;
		updateWordResult.Links = new[]
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
				HRef = $"{GetBaseApiPath()}/dictionaries/{word.DictionaryId}"
			}
		};
		
		// TODO: Scan phrases/etc. for instances of Word within this language and link
		
		return Ok(updateWordResult);
		
		
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