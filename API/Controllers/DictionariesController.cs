using API.Contract;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// Dictionary operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DictionariesController : BaseApiController
{
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly ILanguageRepository _languageRepository;


	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="dictionaryRepository">Implementation of <seealso cref="IDictionaryRepository"/>.</param>
	/// <param name="languageRepository">Implementation of <seealso cref="ILanguageRepository"/>.</param>
	public DictionariesController(IDictionaryRepository dictionaryRepository,
		ILanguageRepository languageRepository)
	{
		_dictionaryRepository = dictionaryRepository;
		_languageRepository = languageRepository;
	}


	/// <summary>
	/// Retrieve matching Dictionaries.
	/// </summary>
	/// <param name="id">If specified, limits results to the matching Dictionary ID.</param>
	/// <param name="ietfLanguageTag">If specified, limits results to the matching Language.</param>
	/// <param name="offsetIndex">If specified, returns results starting at the specified offset position (starting index 0) Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <param name="pageSize">If specified, limits the number of results to the specified limit. Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <response code="200">Results prepared.</response>
	/// <response code="403">Not permitted.</response>
	[HttpGet()]
	[Authorize(Roles="administrator, dataExporter")]
	public async Task<ActionResult<GetDictionariesResult>> GetDictionaries(int? id, string? ietfLanguageTag, int offsetIndex=Defaults.OffsetIndex, int pageSize = Defaults.MaxItems)
	{
		AssertApiConstraints(pageSize);
		
		IEnumerable<Dictionary> words = (await _dictionaryRepository.GetDictionariesAsync(id,ietfLanguageTag)).ToArray();

		string queryString = BuildQueryString(id,ietfLanguageTag);
		GetDictionariesResult getDictionariesResult = new GetDictionariesResult()
		{
			Results = words.Skip(offsetIndex).Take(pageSize).Select(d => new GetDictionaryResultItem()
			{
				Id = d.Id,
				Description = d.Description,
				Name = d.Name,
				CreatedAt = d.CreatedAt,
				CreatedOn = d.CreatedOn,
				SourceUrl = d.SourceUrl,
				IetfLanguageTag = d.IetfLanguageTag,
				CreatedByUserName = d.CreatedByUserName,
				Links = new[]
				{
					new Link()
					{
						Action = "get",
						Rel = "self",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/dictionaries/{d.Id}"
					},
					new Link()
					{
						Action = "get",
						Rel = "language",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/languages/{d.IetfLanguageTag}"
					},
					new Link()
					{
						Action = "get",
						Rel = "firstwords",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/words?dictionaryId={d.Id}"
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
					HRef = $"{GetBaseApiPath()}/dictionaries?{queryString}offsetIndex={offsetIndex}&pageSize={pageSize}"
				}
			},
			FromIndex = offsetIndex,
			PageSize = pageSize,
			TotalItemsCount = words.Count()
		};
		return getDictionariesResult;
	}

	private string BuildQueryString(int? id, string? ietfLanguageTag)
	{
		StringBuilder sb = new StringBuilder();
		if (id.HasValue) sb.Append($"id={id.Value}");
		if (sb.Length > 0) sb.Append("&");
		if (!string.IsNullOrWhiteSpace(ietfLanguageTag)) sb.Append($"ietfLanguageTag={ietfLanguageTag}");
		if (sb.Length > 0) sb.Append("&");
		return sb.ToString();
		
	}


	/// <summary>
	/// Creates a new Dictionary.
	/// </summary>
	/// <param name="createDictionary">A <see cref="CreateDictionary"/> representing the Dictionary to create.</param>
	/// <returns>The created Language.</returns>
	/// <response code="201">Dictionary was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost]
	[Authorize(Roles="administrator,dataImporter")]
	public async Task<ActionResult<CreateDictionaryResult>> CreateDictionary(CreateDictionary createDictionary)
	{
		Language? language = await _languageRepository.GetLanguageByIetfLanguageTagAsync(createDictionary.IetfLanguageTag);
		if (language == null) return BadRequest("Invalid Language");
		
		Dictionary newDictionary = new Dictionary()
		{
			CreatedAt = DateTime.Now,
			CreatedOn = GetRemoteHostAddress(),
			CreatedByUserName = GetCurrentUserName(),
			Name = createDictionary.Name,
			IetfLanguageTag = language.IetfLanguageTag,
			Description = createDictionary.Description,
			SourceUrl = createDictionary.SourceUrl
		};

		_dictionaryRepository.Create(newDictionary);
		if (!await _dictionaryRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/dictionaries/{newDictionary.Id}";
		CreateDictionaryResult createDictionaryResult = new CreateDictionaryResult()
		{
			Id = newDictionary.Id,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new[] { JSON_MIME_TYPE },
					HRef = url
				},
				new Link()
				{
					Action = "get",
					Rel = "language",
					Types = new[] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/languages/{newDictionary.IetfLanguageTag}" 
				}
			}
		};
		
		return Created(url,createDictionaryResult);
	}

}