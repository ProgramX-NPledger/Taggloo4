using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Contract;
using Taggloo4.Dto;
using Taggloo4.Model;
using Taggloo4.Web.Controllers;

namespace Taggloo4.Web.Areas.Api.Controllers;

/// <summary>
/// Translation operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TranslationsController : BaseApiController
{
	private readonly IWordRepository _wordRepository;
	private readonly IPhraseRepository _phraseRepository;
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly ITranslationRepository _translationRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="wordRepository">Implementation of <see cref="IWordRepository"/>.</param>
	/// <param name="phraseRepository">Implementation of <see cref="IPhraseRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	/// <param name="translationRepository">Implementation of <see cref="ITranslationRepository"/>.</param>
	public TranslationsController(IWordRepository wordRepository, 
		IPhraseRepository phraseRepository,
		IDictionaryRepository dictionaryRepository,
		ITranslationRepository translationRepository)
	{
		_wordRepository = wordRepository;
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
		_translationRepository = translationRepository;
	}
	
	// TODO: Look at deprecating this controller nad moving into WordTranslationsController
	
	
    /// <summary>
	/// Creates a new Phrase Translation.
	/// </summary>
	/// <param name="createPhraseTranslation">A <see cref="CreatePhraseTranslation"/> representing the Translation to create.</param>
	/// <returns>The created Translation.</returns>
	/// <response code="201">Phrase Translation was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost("phrase")] // /api/v4/translations/phrase
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<CreatePhraseTranslationResult>> CreatePhraseTranslation(CreatePhraseTranslation createPhraseTranslation)
	{
		// try to resolve the dictionary
		Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(createPhraseTranslation.DictionaryId);
		if (dictionary == null) return BadRequest("Invalid Dictionary");
		
		// do the words exist?
		Phrase? fromPhrase = await _phraseRepository.GetByIdAsync(createPhraseTranslation.FromPhraseId);
		if (fromPhrase == null) return BadRequest("From Phrase does not exist");
		Phrase? toWord = await _phraseRepository.GetByIdAsync(createPhraseTranslation.ToPhraseId);
		if (toWord == null) return BadRequest("To Phrase does not exist");

		PhraseTranslation phraseTranslation=new PhraseTranslation()
		{
			CreatedAt = createPhraseTranslation.CreatedAt ?? DateTime.Now,
			CreatedOn = createPhraseTranslation.CreatedOn ?? GetRemoteHostAddress(),
			CreatedByUserName = createPhraseTranslation.CreatedByUserName ?? GetCurrentUserName(),
			DictionaryId = createPhraseTranslation.DictionaryId,
			FromPhraseId = createPhraseTranslation.FromPhraseId,
			ToPhraseId = createPhraseTranslation.ToPhraseId
		};

		_translationRepository.Create(phraseTranslation);
		if (!await _wordRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/translations/phrase/{phraseTranslation.Id}";
		CreatePhraseTranslationResult createPhraseTranslationResult = new CreatePhraseTranslationResult()
		{
			Id = phraseTranslation.Id,
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
					HRef = $"{GetBaseApiPath()}/dictionaries/{phraseTranslation.DictionaryId}"
				},
				new Link()
				{
					Action="get",
					Rel="fromPhrase",
					Types = new [] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/phrases/{phraseTranslation.FromPhraseId}"
				},
				new Link()
				{
					Action="get",
					Rel="toPhrase",
					Types=new [] { JSON_MIME_TYPE },
					HRef=$"{GetBaseApiPath()}/words/{phraseTranslation.ToPhraseId}"
				}
			}
		};
		
		return Created(url,createPhraseTranslationResult);
		
		
	}
    
	/// <summary>
	/// Update an existing Phrase Translation with meta-data.
	/// </summary>
	/// <param name="phraseTranslationId">Identifier of Word Translation to update.</param>
	/// <param name="updatePhraseTranslation">A <seealso cref="UpdatePhraseTranslation"/> representing the Phrase Translation to update.</param>
	/// <returns>The updated Word.</returns>
	/// <response code="200">Word was updated.</response>
	/// <response code="400">One or more validation errors prevented successful updating.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPatch("phrase/{phraseTranslationId}")] // /api/v4/translations/word
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult> UpdatePhraseTranslation(int phraseTranslationId, UpdatePhraseTranslation updatePhraseTranslation)
	{
		// does the translation exist?
		PhraseTranslation? phraseTranslation = await _translationRepository.GetPhraseTranslationByIdAsync(phraseTranslationId);
		if (phraseTranslation == null) return NotFound();
		

		
		// update phrase translation
		UpdatePhraseTranslationResult updatePhraseTranslationResult = new UpdatePhraseTranslationResult()
		{

		};

		if (updatePhraseTranslation.FromPhraseId.HasValue) phraseTranslation.FromPhraseId = updatePhraseTranslation.FromPhraseId.Value;
		if (updatePhraseTranslation.ToPhraseId.HasValue) phraseTranslation.ToPhraseId = updatePhraseTranslation.ToPhraseId.Value;
		if (updatePhraseTranslation.DictionaryId.HasValue)
		{
			// does the new dictionary exist?
			Dictionary? newDictionary = await _dictionaryRepository.GetByIdAsync(updatePhraseTranslation.DictionaryId.Value);
			if (newDictionary == null) return BadRequest("Invalid attempt to move Phrase into non-existent Dictionary");
			
			// is the new dictionary the same language as the previous?
			Dictionary? oldDictionary = await _dictionaryRepository.GetByIdAsync(phraseTranslation.DictionaryId);

			if (!newDictionary.IetfLanguageTag.Equals(oldDictionary!.IetfLanguageTag))
			{
				return BadRequest("Invalid attempt to move Phrase into Dictionary of another Language");
			}

			phraseTranslation.DictionaryId = updatePhraseTranslation.DictionaryId.Value;
			updatePhraseTranslationResult.RequiresDictionaryReindexing = true;
		}

		if (updatePhraseTranslation.CreatedOn != null) phraseTranslation.CreatedOn = updatePhraseTranslation.CreatedOn;
		if (updatePhraseTranslation.CreatedByUserName != null) phraseTranslation.CreatedByUserName = updatePhraseTranslation.CreatedByUserName;
		if (updatePhraseTranslation.CreatedAt.HasValue) phraseTranslation.CreatedAt = updatePhraseTranslation.CreatedAt.Value;
		
		_translationRepository.Update(phraseTranslation);
		if (!await _translationRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/translations/phrase/{phraseTranslation.Id}";
		updatePhraseTranslationResult.Id = phraseTranslation.Id;
		updatePhraseTranslationResult.Links = new[]
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
				HRef = $"{GetBaseApiPath()}/dictionaries/{phraseTranslation.DictionaryId}"
			},
			new Link()
			{
				Action="get",
				Rel="fromWord",
				Types = new [] { JSON_MIME_TYPE },
				HRef = $"{GetBaseApiPath()}/phrases/{phraseTranslation.FromPhraseId}"
			},
			new Link()
			{
				Action="get",
				Rel="toWord",
				Types=new [] { JSON_MIME_TYPE },
				HRef=$"{GetBaseApiPath()}/phrases/{phraseTranslation.ToPhraseId}"
			}
		};
		
		return Ok(updatePhraseTranslationResult);
		
		
	}

	

	
}