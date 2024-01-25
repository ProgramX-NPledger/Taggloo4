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

	[HttpGet("{word}")]
	[Authorize(Roles="administrator, dataExporter")]
	public async Task<ActionResult<GetWordsResult>> GetWords(string word, int? dictionaryId, int offsetIndex=Defaults.OffsetIndex, int pageSize = Defaults.MaxItems)
	{
		AssertApiConstraints(pageSize);
		
		IEnumerable<Word> words = (await _wordRepository.GetWords(word, dictionaryId)).ToArray();

		GetWordsResult getWordsResult = new GetWordsResult()
		{
			Results = words.Skip(offsetIndex).Take(pageSize).Select(w => new GetWordResultItem()
			{
				Id = w.Id,
				Word = w.TheWord,
				Links = new[]
				{
					new Link()
					{
						Action = "get",
						Rel = "self",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/words/{w.Id}"
					},
					new Link()
					{
						Action = "get",
						Rel = "dictionary",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/dictionary/{w.DictionaryId}"
					}
				}
			}),
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new[] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/api/v4/words/{word}?offsetIndex={offsetIndex}&pageSize={pageSize}"
				}
			},
			FromIndex = offsetIndex,
			PageSize = pageSize,
			TotalItemsCount = words.Count()
		};
		return getWordsResult;
	}
	
	
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
		IEnumerable<Word> existingWord = await _wordRepository.GetWords(createWord.Word, dictionary.Id);
		if (existingWord.Any())
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
			IEnumerable<Word>? newWord = await _wordRepository.GetWords(updateWord.Word,updateWord.DictionaryId);
			if (newWord.Any()) return BadRequest("The word is being renamed to another Word that already exists within the Dictionary");

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

	
}