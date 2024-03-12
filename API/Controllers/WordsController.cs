using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.Helper;
using API.Jobs;
using API.Model;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WordsController : BaseApiController
{
	private readonly IWordRepository _wordRepository;
	private readonly IPhraseRepository _phraseRepository;
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly IBackgroundJobClient _backgroundJobClient;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="wordRepository">Implementation of <see cref="IWordRepository"/>.</param>
	/// <param name="phraseRepository">Implementation of <see cref="IPhraseRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	/// <param name="backgroundJobClient">Implementation of <see cref="IBackgroundJobClient" />.</param>
	public WordsController(IWordRepository wordRepository,
		IPhraseRepository phraseRepository,
		IDictionaryRepository dictionaryRepository,
		IBackgroundJobClient backgroundJobClient)
	{
		_wordRepository = wordRepository;
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
		_backgroundJobClient = backgroundJobClient;
	}

	[HttpGet("{importGuid}/importguid")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetWordResultItem>> GetWordByImportGuid(Guid importGuid)
	{
		Word? word = await _wordRepository.GetByImportIdAsync(importGuid);
		if (word == null) return NotFound();

		return Ok(new GetWordResultItem()
		{
			Word = word.TheWord,
			Id = word.Id,
			CreatedAt = word.CreatedAt,
			CreatedOn = word.CreatedOn,
			CreatedByUserName = word.CreatedByUserName,
			DictionaryId = word.DictionaryId,
			ImportId = word.ImportId,
			IetfLanguageTag = word.Dictionary?.IetfLanguageTag,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/words/{word.Id}",
					Types = new[] { JSON_MIME_TYPE }
				}
			}
		});
	}

	[HttpGet("{id}")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetWordResultItem>> GetWordById(int id)
	{
		Word? word = await _wordRepository.GetByIdAsync(id);
		if (word == null) return NotFound();

		return Ok(new GetWordResultItem()
		{
			Word = word.TheWord,
			Id = word.Id,
			CreatedAt = word.CreatedAt,
			CreatedOn = word.CreatedOn,
			CreatedByUserName = word.CreatedByUserName,
			DictionaryId = word.DictionaryId,
			ImportId = word.ImportId,
			IetfLanguageTag = word.Dictionary?.IetfLanguageTag,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/words/{word.Id}",
					Types = new[] { JSON_MIME_TYPE }
				}
			}
		});
	}

	
	/// <summary>
	/// Retrieve matching Words from an optional Dictionary.
	/// </summary>
	/// <param name="word">The word to search for.</param>
	/// <param name="dictionaryId">If specified, searches within the Dictionary represented by the ID.</param>
	/// <param name="offsetIndex">If specified, returns results starting at the specified offset position (starting index 0) Default is defined by <see cref="Defaults.OffsetIndex"/>.</param>
	/// <param name="pageSize">If specified, limits the number of results to the specified limit. Default is defined by <see cref="Defaults.MaxItems" />.</param>
	/// <response code="200">Results prepared.</response>
	/// <response code="403">Not permitted.</response>
	[HttpGet()]
	[Authorize(Roles="administrator, dataExporter")]
	public async Task<ActionResult<GetWordsResult>> GetWords(string? word, int? dictionaryId, int offsetIndex=Defaults.OffsetIndex, int pageSize = Defaults.MaxItems)
	{
		AssertApiConstraints(pageSize);
		
		IEnumerable<Word> words = (await _wordRepository.GetWordsAsync(word, dictionaryId)).ToArray();

		GetWordsResult getWordsResult = new GetWordsResult()
		{
			Results = words.Skip(offsetIndex).Take(pageSize).Select(w => new GetWordResultItem()
			{
				Id = w.Id,
				Word = w.TheWord,
				CreatedAt = w.CreatedAt,
				CreatedOn = w.CreatedOn,
				CreatedByUserName = w.CreatedByUserName,
				DictionaryId = w.DictionaryId,
				ImportId = w.ImportId,
				IetfLanguageTag = w.Dictionary?.IetfLanguageTag,
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
						HRef = $"{GetBaseApiPath()}/dictionaries/{w.DictionaryId}"
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
					HRef = $"{GetBaseApiPath()}/words?word={word}&offsetIndex={offsetIndex}&pageSize={pageSize}"
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
	/// <response code="202">Word creation was accepted.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost]
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> CreateWord(CreateWord createWord)
	{
		Thread.Sleep(1000);
		
		// try to resolve the dictionary
		Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(createWord.DictionaryId);
		if (dictionary == null) return BadRequest("Invalid Dictionary");

		Guid importGuid = Guid.NewGuid();
// #if !USE_HANGFIRE
// 			ImportWordJob importWordJob = new ImportWordJob(_wordRepository, _phraseRepository, _dictionaryRepository);
// 			importWordJob.ImportWord(createWord.CreatedOn ?? GetRemoteHostAddress(), 
// 				createWord.CreatedByUserName ?? GetCurrentUserName(), createWord.Word, dictionary.Id,importGuid,createWord.CreatedAt);
// #else
// 			_backgroundJobClient.Enqueue<ImportWordJob>(job =>
// 				job.ImportWord(createWord.CreatedOn ?? GetRemoteHostAddress(), 
// 					createWord.CreatedByUserName ?? GetCurrentUserName(), createWord.Word, dictionary.Id,importGuid,createWord.CreatedAt));
// #endif


		return Accepted(new CreateWordResult()
		{
			ImportId = importGuid,
			Links = new []
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/words/{importGuid}/importguid",
					Types = new []{ JSON_MIME_TYPE }
				}
			}
		});
		
	}



	/// <summary>
	/// Update an existing Word with meta-data.
	/// </summary>
	/// <param name="updateWord">A <see cref="UpdateWord"/> representing the Word to update.</param>
	/// <returns>The updated Word.</returns>
	/// <response code="200">Word was updated.</response>
	/// <response code="400">One or more validation errors prevented successful updating.</response>
	/// <response code="403">Not permitted.</response>
	/// <response code="404">Word not found.</response>
	[HttpPatch]
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> UpdateWord(UpdateWord updateWord)
	{
		Word? word= await _wordRepository.GetByIdAsync(updateWord.WordId);
		if (word == null) return NotFound();
		
		// update word
		UpdateWordResult updateWordResult = new UpdateWordResult()
		{

		};

		if (updateWord.MoveWordToDictionaryId.HasValue &&
		    word.DictionaryId != updateWord.MoveWordToDictionaryId.Value)
		{
			// does the new dictionary exist?
			Dictionary? newDictionary = await _dictionaryRepository.GetByIdAsync(updateWord.MoveWordToDictionaryId.Value);
			if (newDictionary == null) return BadRequest("Invalid attempt to move Word into non-existent Dictionary");
			
			// is the new dictionary the same language as the previous?
			Dictionary? oldDictionary = await _dictionaryRepository.GetByIdAsync(word.DictionaryId);

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
		
		return Ok(updateWordResult);
		
		
	}

	
}