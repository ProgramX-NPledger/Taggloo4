using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Dto;
using Taggloo4.Model;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Controllers;

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
	
    /// <summary>
	/// Creates a new Word Translation.
	/// </summary>
	/// <param name="createWordTranslation">A <see cref="CreateWordTranslation"/> representing the Translation to create.</param>
	/// <returns>The created Translation.</returns>
	/// <response code="201">Word Translation was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost("word")] // /api/v4/translations/word
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> CreateWordTranslation(CreateWordTranslation createWordTranslation)
	{
		// try to resolve the dictionary
		Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(createWordTranslation.DictionaryId);
		if (dictionary == null) return BadRequest("Invalid Dictionary");
		
		// do the words exist?
		Word? fromWord = await _wordRepository.GetByIdAsync(createWordTranslation.FromWordId);
		if (fromWord == null) return BadRequest("From Word does not exist");
		Word? toWord = await _wordRepository.GetByIdAsync(createWordTranslation.ToWordId);
		if (toWord == null) return BadRequest("To Word does not exist");

		WordTranslation wordTranslation=new WordTranslation()
		{
			CreatedAt = createWordTranslation.CreatedAt ?? DateTime.Now,
			CreatedOn = createWordTranslation.CreatedOn ?? GetRemoteHostAddress(),
			CreatedByUserName = createWordTranslation.CreatedByUserName ?? GetCurrentUserName(),
			DictionaryId = createWordTranslation.DictionaryId,
			FromWordId = createWordTranslation.FromWordId,
			ToWordId = createWordTranslation.ToWordId
		};

		_translationRepository.Create(wordTranslation);
		if (!await _wordRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/translations/word/{wordTranslation.Id}";
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
	/// Update an existing Word Translation with meta-data.
	/// </summary>
	/// <param name="wordTranslationId">Identifier of Word Translation to update.</param>
	/// <param name="updateWordTranslation">A <seealso cref="UpdateWordTranslation"/> representing the Word Translation to update.</param>
	/// <returns>The updated Word.</returns>
	/// <response code="200">Word was updated.</response>
	/// <response code="400">One or more validation errors prevented successful updating.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPatch("word/{wordTranslationId}")] // /api/v4/translations/word
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<AppUser>> UpdateWordTranslation(int wordTranslationId, UpdateWordTranslation updateWordTranslation)
	{
		// does the translation exist?
		WordTranslation? wordTranslation = await _translationRepository.GetWordTranslationByIdAsync(wordTranslationId);
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
			Dictionary? newDictionary = await _dictionaryRepository.GetByIdAsync(updateWordTranslation.DictionaryId.Value);
			if (newDictionary == null) return BadRequest("Invalid attempt to move Word into non-existent Dictionary");
			
			// is the new dictionary the same language as the previous?
			Dictionary? oldDictionary = await _dictionaryRepository.GetByIdAsync(wordTranslation.DictionaryId);

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
	public async Task<ActionResult<AppUser>> UpdatePhraseTranslation(int phraseTranslationId, UpdatePhraseTranslation updatePhraseTranslation)
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