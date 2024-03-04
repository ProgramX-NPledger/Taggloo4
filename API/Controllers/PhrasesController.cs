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
public class PhrasesController : BaseApiController
{
	private readonly IPhraseRepository _phraseRepository;
	private readonly IDictionaryRepository _dictionaryRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="phraseRepository">Implementation of <seealso cref="IPhraseRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <seealso cref="IDictionaryRepository"/>.</param>
	public PhrasesController(IPhraseRepository phraseRepository, 
		IDictionaryRepository dictionaryRepository)
	{
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
		
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
		
		IEnumerable<Phrase> words = (await _phraseRepository.GetPhrasesAsync(phrase, dictionaryId)).ToArray();

		GetPhrasesResult getPhrasesResult = new GetPhrasesResult()
		{
			Results = words.Skip(offsetIndex).Take(pageSize).Select(p => new GetPhrasesResultItem()
			{
				Id = p.Id,
				Phrase = p.ThePhrase,
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
	public async Task<ActionResult<AppUser>> CreatePhrase(CreatePhrase createPhrase)
	{
		// try to resolve the dictionary
		Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(createPhrase.DictionaryId);
		if (dictionary == null) return BadRequest("Invalid Dictionary");
		
		// does the Phrase for the language already exist? If so, reject - maybe a translation is required
		IEnumerable<Phrase> existingPhrase = await _phraseRepository.GetPhrasesAsync(createPhrase.Phrase, dictionary.Id);
		if (existingPhrase.Any())
			return BadRequest("Phrase already exists, perhaps a Translation from the existing Phrase is appropriate?");
		
		Phrase newPhrase = new Phrase()
		{
			CreatedAt = DateTime.Now,
			CreatedOn = GetRemoteHostAddress(),
			CreatedByUserName = GetCurrentUserName(),
			ThePhrase = createPhrase.Phrase,
			DictionaryId = dictionary.Id
		};

		_phraseRepository.Create(newPhrase);
		if (!await _phraseRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/phrases/{newPhrase.Id}";
		CreatePhraseResult createPhraseResult = new CreatePhraseResult()
		{
			Id = newPhrase.Id,
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
					HRef = $"{GetBaseApiPath()}/dictionaries/{newPhrase.DictionaryId}"
				}
			}
		};
		
		// TODO: Scan phrases/etc. for instances of Word within this language and link
		// TODO: WordsinPhrase
		
		return Created(url,createPhraseResult);
		
		
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

		if (updatePhrase.Phrase!=null &&
		    !phrase.ThePhrase.Equals(updatePhrase.Phrase))
		{
			// changing the word is dangerous. Ensure the new word doesn't already exist
			IEnumerable<Phrase>? newPhrase = await _phraseRepository.GetPhrasesAsync(updatePhrase.Phrase,updatePhrase.DictionaryId);
			if (newPhrase.Any()) return BadRequest("The Phrase is being renamed to another Word that already exists within the Dictionary");

			phrase.ThePhrase = updatePhrase.Phrase;
		}

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
		
		// TODO: Scan phrases/etc. for instances of Word within this language and link
		
		return Ok(updatePhraseResult);
		
		
	}

	
}