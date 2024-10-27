using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Contract;
using Taggloo4.Dto;
using Taggloo4.Model;
using Taggloo4.Web.Controllers;

namespace Taggloo4.Web.Areas.Api.Controllers;

/// <summary>
/// Word Translation operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WordTranslationsController : BaseApiController
{
	private readonly ITranslationRepository _translationRepository;
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly IWordRepository _wordRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="translationRepository">Implementation of <see cref="ITranslationRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	/// <param name="wordRepository">Implementation of <seealso cref="IWordRepository"/>.</param>
	public WordTranslationsController(ITranslationRepository translationRepository,
		IDictionaryRepository dictionaryRepository,
		IWordRepository wordRepository)
	{
		_translationRepository = translationRepository;
		_dictionaryRepository = dictionaryRepository;
		_wordRepository = wordRepository;
	}


	/// <summary>
	/// Retrieves the specified Word Translation.
	/// </summary>
	/// <param name="id">ID of the Word Translation.</param>
	/// <response code="200">Word Translation found.</response>
	/// <response code="404">Word Translation not found.</response>
	[HttpGet("{id}")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetWordTranslationResultItem>> GetWordTranslationById(int id)
	{
		WordTranslation? wordTranslation = await _translationRepository.GetWordTranslationByIdAsync(id);
		if (wordTranslation == null) return NotFound();

		List<Link> links = new List<Link>()
		{
			new Link()
			{
				Action = "get",
				Rel = "self",
				HRef = $"{GetBaseApiPath()}/wordtranslations/{wordTranslation.Id}",
				Types = new[] { JSON_MIME_TYPE }
			},
			new Link()
			{
				Action = "get",
				Rel = "dictionary",
				HRef = $"{GetBaseApiPath()}/dictionaries/{wordTranslation.DictionaryId}",
				Types = new[] { JSON_MIME_TYPE }
			},
			new Link()
			{
				Action = "get",
				Rel = "fromWord",
				HRef = $"{GetBaseApiPath()}/words/{wordTranslation.FromWordId}",
				Types = new[] { JSON_MIME_TYPE }
			},
			new Link()
			{
				Action = "get",
				Rel = "toWord",
				HRef = $"{GetBaseApiPath()}/words/{wordTranslation.ToWordId}",
				Types = new[] { JSON_MIME_TYPE }
			}
		};

		if (wordTranslation.FromWord != null && wordTranslation.FromWord.Dictionaries!=null)
		{
			foreach (Dictionary dictionary in wordTranslation.FromWord.Dictionaries)
			{
				links.Add(new Link()
				{
					Action = "get",
					Rel = "wordDictionary",
					HRef = $"{GetBaseApiPath()}/dictionaries/{dictionary.Id}",
					Types = new[] { JSON_MIME_TYPE }
				});
			}
			
		}
		
		return Ok(new GetWordTranslationResultItem()
		{
			Id = wordTranslation.Id,
			CreatedAt = wordTranslation.CreatedAt,
			CreatedOn = wordTranslation.CreatedOn,
			CreatedByUserName = wordTranslation.CreatedByUserName,
			DictionaryId = wordTranslation.DictionaryId,
			FromWord = wordTranslation.FromWord?.TheWord,
			FromWordId = wordTranslation.FromWordId,
			ToWordId = wordTranslation.ToWordId,
			Links = links
		});
	}

	
    /// <summary>
	/// Creates a new Word Translation.
	/// </summary>
	/// <param name="createWordTranslation">A <see cref="CreateWordTranslation"/> representing the Translation to create.</param>
	/// <returns>The created Translation.</returns>
	/// <response code="201">Word Translation was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost()] // /api/v4/wordtranslations
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult> CreateWordTranslation(CreateWordTranslation createWordTranslation)
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
	[HttpPatch("{wordTranslationId}")] // /api/v4/wordtranslations
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult> UpdateWordTranslation(int wordTranslationId, UpdateWordTranslation updateWordTranslation)
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
	
	
}