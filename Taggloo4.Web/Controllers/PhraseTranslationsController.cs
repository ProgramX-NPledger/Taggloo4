using System.Collections.ObjectModel;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Contract;
using Taggloo4.Dto;
using Taggloo4.Model;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Controllers;

/// <summary>
/// Phrase Translation operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PhraseTranslationsController : BaseApiController
{
	private readonly ITranslationRepository _translationRepository;
	private readonly IDictionaryRepository _dictionaryRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="translationRepository">Implementation of <see cref="ITranslationRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	public PhraseTranslationsController(ITranslationRepository translationRepository,
		IDictionaryRepository dictionaryRepository)
	{
		_translationRepository = translationRepository;
		_dictionaryRepository = dictionaryRepository;
	}


	/// <summary>
	/// Retrieves the specified Phrase Translation.
	/// </summary>
	/// <param name="id">ID of the Phrase Translation.</param>
	/// <response code="200">Phrase Translation found.</response>
	/// <response code="404">Phrase Translation not found.</response>
	[HttpGet("{id}")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetPhraseTranslationResultItem>> GetPhraseTranslationById(int id)
	{
		PhraseTranslation? phraseTranslation = await _translationRepository.GetPhraseTranslationByIdAsync(id);
		if (phraseTranslation == null) return NotFound();

		return Ok(new GetPhraseTranslationResultItem()
		{
			Id = phraseTranslation.Id,
			CreatedAt = phraseTranslation.CreatedAt,
			CreatedOn = phraseTranslation.CreatedOn,
			CreatedByUserName = phraseTranslation.CreatedByUserName,
			DictionaryId = phraseTranslation.DictionaryId,
			FromPhrase = phraseTranslation.FromPhrase?.ThePhrase,
			FromPhraseId = phraseTranslation.FromPhraseId,
			FromIetfLanguageTag = phraseTranslation.FromPhrase?.Dictionary?.IetfLanguageTag,
			ToPhraseId = phraseTranslation.ToPhraseId,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/phrasetranslations/{phraseTranslation.Id}",
					Types = new[] { JSON_MIME_TYPE }
				},
				new Link()
				{
					Action = "get",
					Rel = "dictionary",
					HRef = $"{GetBaseApiPath()}/dictionaries/{phraseTranslation.DictionaryId}",
					Types = new[] { JSON_MIME_TYPE }
				},
				new Link()
				{
					Action = "get",
					Rel = "fromWord",
					HRef = $"{GetBaseApiPath()}/phrases/{phraseTranslation.FromPhraseId}",
					Types = new[] { JSON_MIME_TYPE }
				},
				new Link()
				{
					Action="get",
					Rel = "toWord",
					HRef = $"{GetBaseApiPath()}/phrases/{phraseTranslation.ToPhraseId}",
					Types = new[] { JSON_MIME_TYPE }
				}
			}
		});
	}

	
}