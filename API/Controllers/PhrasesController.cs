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
	private readonly IDictionaryRepository _dictionaryRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="phraseRepository">Implementation of <see cref="IPhraseRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	public PhrasesController(IPhraseRepository phraseRepository,
		IDictionaryRepository dictionaryRepository)
	{
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
	}
	
	
	[HttpGet("{importGuid}/externalid")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetPhrasesResult>> GetPhraseByExternalId(string externalId)
	{
		return await GetPhrases(null, null, externalId);
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
			ExternalId = phrase.ExternalId,
			IetfLanguageTag = phrase.Dictionary?.IetfLanguageTag,
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
	public async Task<ActionResult<GetPhrasesResult>> GetPhrases(string? phrase, 
		int? dictionaryId,
		string? externalId,
		int offsetIndex=Defaults.OffsetIndex, 
		int pageSize = Defaults.MaxItems)
	{
		AssertApiConstraints(pageSize);
		
		IEnumerable<Phrase> words = (await _phraseRepository.GetPhrasesAsync(phrase, dictionaryId,null, externalId)).ToArray();

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
				ExternalId = p.ExternalId,
				IetfLanguageTag = p.Dictionary?.IetfLanguageTag,
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
		
		IEnumerable<Phrase> existingPhrases = await _phraseRepository.GetPhrasesAsync(createPhrase.Phrase,createPhrase.DictionaryId,null,null);
		if (existingPhrases.Any()) return BadRequest("Phrase already exists in Dictionary");

		Phrase newPhrase = new Phrase()
		{
			CreatedAt = createPhrase.CreatedAt ?? DateTime.Now,
			CreatedOn = createPhrase.CreatedOn ?? GetRemoteHostAddress(),
			CreatedByUserName = createPhrase.CreatedByUserName ?? GetCurrentUserName(),
			ThePhrase = createPhrase.Phrase,
			DictionaryId = createPhrase.DictionaryId,
			ExternalId = createPhrase.ExternalId,
			Words = new Collection<Word>()
		};

		_phraseRepository.Create(newPhrase);
		if (!await _phraseRepository.SaveAllAsync()) return BadRequest();

		string url = $"{GetBaseApiPath()}/phrases/{newPhrase.Id}";
		return Created(url,new CreatePhraseResult()
		{
			ExternalId = newPhrase.ExternalId,
			PhraseId = newPhrase.Id,
			RequiresReindexing = true,
			Links = new []
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = url,
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