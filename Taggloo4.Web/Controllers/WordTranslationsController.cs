﻿using System.Collections.ObjectModel;
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
/// Word Translation operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WordTranslationsController : BaseApiController
{
	private readonly ITranslationRepository _translationRepository;
	private readonly IDictionaryRepository _dictionaryRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="translationRepository">Implementation of <see cref="ITranslationRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	public WordTranslationsController(ITranslationRepository translationRepository,
		IDictionaryRepository dictionaryRepository)
	{
		_translationRepository = translationRepository;
		_dictionaryRepository = dictionaryRepository;
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

	
}