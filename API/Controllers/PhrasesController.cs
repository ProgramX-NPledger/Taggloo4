using System.Collections.ObjectModel;
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
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PhrasesController : BaseApiController
{
	private readonly IPhraseRepository _phraseRepository;
	private readonly IWordRepository _wordRepository;
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly IBackgroundJobClient _backgroundJobClient;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="phraseRepository">Implementation of <see cref="IPhraseRepository"/>.</param>
	/// <param name="wordRepository">Implementation of <see cref="IWordRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	/// <param name="backgroundJobClient">Implementation of <see cref="IBackgroundJobClient"/></param>
	public PhrasesController(IPhraseRepository phraseRepository,
		IWordRepository wordRepository,
		IDictionaryRepository dictionaryRepository,
		IBackgroundJobClient backgroundJobClient)
	{
		_phraseRepository = phraseRepository;
		_wordRepository = wordRepository;
		_dictionaryRepository = dictionaryRepository;
		_backgroundJobClient = backgroundJobClient;
	}
	
	
	[HttpGet("{importGuid}/importguid")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetPhraseResultItem>> GetPhraseByImportGuid(Guid importGuid)
	{
		Phrase? phrase = await _phraseRepository.GetByImportIdAsync(importGuid);
		if (phrase == null) return NotFound();

		return Ok(new GetPhraseResultItem()
		{
			Phrase = phrase.ThePhrase,
			Id = phrase.Id,
			CreatedAt = phrase.CreatedAt,
			CreatedOn = phrase.CreatedOn,
			CreatedByUserName = phrase.CreatedByUserName,
			DictionaryId = phrase.DictionaryId,
			ImportId = phrase.ImportId,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/phrases/{phrase.Id}",
					Types = new[] { JSON_MIME_TYPE }
				}
			}
		});
	}

	[HttpGet("{id}")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetWordResultItem>> GetPhraseById(int id)
	{
		Phrase? phrase = await _phraseRepository.GetByIdAsync(id);
		if (phrase == null) return NotFound();

		return Ok(new GetPhraseResultItem()
		{
			Phrase = phrase.ThePhrase,
			Id = phrase.Id,
			CreatedAt = phrase.CreatedAt,
			CreatedOn = phrase.CreatedOn,
			CreatedByUserName = phrase.CreatedByUserName,
			DictionaryId = phrase.DictionaryId,
			ImportId = phrase.ImportId,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/phrases/{phrase.Id}",
					Types = new[] { JSON_MIME_TYPE }
				}
			}
		});
	}
	

	/// <summary>
	/// Retrieve matching Phrases from an optional Dictionary.
	/// </summary>
	/// <param name="phrase">The Phrase to search for.</param>
	/// <param name="dictionaryId">If specified, searches within the Dictionary represented by the ID.</param>
	/// <param name="offsetIndex">If specified, returns results starting at the specified offset position (starting index 0) Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <param name="pageSize">If specified, limits the number of results to the specified limit. Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <response code="200">Results prepared.</response>
	/// <response code="403">Not permitted.</response>
	[HttpGet()]
	[Authorize(Roles="administrator, dataExporter")]
	public async Task<ActionResult<GetPhrasesResult>> GetPhrases(string? phrase, int? dictionaryId, int offsetIndex=Defaults.OffsetIndex, int pageSize = Defaults.MaxItems)
	{
		AssertApiConstraints(pageSize);
		
		IEnumerable<Phrase> words = (await _phraseRepository.GetPhrasesAsync(phrase, dictionaryId,null)).ToArray();

		GetPhrasesResult getPhrasesResult = new GetPhrasesResult()
		{
			Results = words.Skip(offsetIndex).Take(pageSize).Select(p => new GetPhraseResultItem()
			{
				Id = p.Id,
				Phrase = p.ThePhrase,
				CreatedAt = p.CreatedAt,
				CreatedOn = p.CreatedOn,
				CreatedByUserName = p.CreatedByUserName,
				DictionaryId = p.DictionaryId,
				ImportId = p.ImportId,
				Links = new[]
				{
					new Link()
					{
						Action = "get",
						Rel = "self",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/phrases/{p.Id}"
					},
					new Link()
					{
						Action = "get",
						Rel = "dictionary",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/dictionaries/{p.DictionaryId}"
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
					HRef = $"{GetBaseApiPath()}/words?phrase={phrase}&offsetIndex={offsetIndex}&pageSize={pageSize}"
				}
			},
			FromIndex = offsetIndex,
			PageSize = pageSize,
			TotalItemsCount = words.Count()
		};
		return getPhrasesResult;
	}
	
	
    /// <summary>
	/// Creates a new Phrase.
	/// </summary>
	/// <param name="createPhrase">A <see cref="CreatePhrase"/> representing the Phrase to create.</param>
	/// <returns>The created Phrase.</returns>
	/// <response code="201">Phrase was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost]
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<CreatePhraseResult>> CreatePhrase(CreatePhrase createPhrase)
	{
		// try to resolve the dictionary
		Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(createPhrase.DictionaryId);
		if (dictionary == null) return BadRequest("Invalid Dictionary");
		
		Guid importGuid = Guid.NewGuid();
		_backgroundJobClient.Enqueue<ImportPhraseJob>(job =>
			job.ImportPhrase(createPhrase.CreatedOn ?? GetRemoteHostAddress(), 
				createPhrase.CreatedByUserName ?? GetCurrentUserName(), 
				createPhrase.Phrase, 
				dictionary.Id,
				importGuid));

		return Accepted(new CreatePhraseResult()
		{
			ImportId = importGuid,
			Links = new []
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/phrases/{importGuid}/importguid",
					Types = new []{ JSON_MIME_TYPE }
				}
			}
		});
		
		
	}
    
	/// <summary>
	/// Update an existing Phrase with meta-data.
	/// </summary>
	/// <param name="updatePhrase">A <see cref="UpdatePhrase"/> representing the Phrase to update.</param>
	/// <returns>The updated Phrase.</returns>
	/// <response code="200">Phrase was updated.</response>
	/// <response code="400">One or more validation errors prevented successful updating.</response>
	/// <response code="403">Not permitted.</response>
	/// <response code="404">Phrase not found.</response>
	[HttpPatch]
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> UpdatePhrase(UpdatePhrase updatePhrase)
	{
		Phrase? phrase = await _phraseRepository.GetByIdAsync(updatePhrase.PhraseId);
		if (phrase == null) return NotFound();
		
		// update phrase
		UpdatePhraseResult updatePhraseResult = new UpdatePhraseResult()
		{

		};
		
		if (updatePhrase.MovePhraseToDictionaryId.HasValue &&
		    phrase.DictionaryId != updatePhrase.MovePhraseToDictionaryId.Value)
		{
			// does the new dictionary exist?
			Dictionary? newDictionary = await _dictionaryRepository.GetByIdAsync(updatePhrase.MovePhraseToDictionaryId.Value);
			if (newDictionary == null) return BadRequest("Invalid attempt to move Phrase into non-existent Dictionary");
			
			// is the new dictionary the same language as the previous?
			Dictionary? oldDictionary = await _dictionaryRepository.GetByIdAsync(phrase.DictionaryId);

			if (!newDictionary.IetfLanguageTag.Equals(oldDictionary!.IetfLanguageTag))
			{
				return BadRequest("Invalid attempt to move Phrase into Dictionary of another Language");
			}

			phrase.DictionaryId = updatePhrase.MovePhraseToDictionaryId.Value;
			updatePhraseResult.RequiresDictionaryReindexing = true;
		}

		if (updatePhrase.CreatedOn != null) phrase.CreatedOn = updatePhrase.CreatedOn;
		if (updatePhrase.CreatedByUserName != null) phrase.CreatedByUserName = updatePhrase.CreatedByUserName;
		if (updatePhrase.CreatedAt.HasValue) phrase.CreatedAt = updatePhrase.CreatedAt.Value;
		
		_phraseRepository.Update(phrase);
		if (!await _phraseRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/phrases/{phrase.Id}";
		updatePhraseResult.Id = phrase.Id;
		updatePhraseResult.Links = new[]
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
				HRef = $"{GetBaseApiPath()}/dictionaries/{phrase.DictionaryId}"
			}
		};
		
		return Ok(updatePhraseResult);
		
		
	}

	
}