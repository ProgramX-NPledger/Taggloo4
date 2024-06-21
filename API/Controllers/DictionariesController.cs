using API.Contract;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using API.Jobs;
using Hangfire;
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
	private readonly IBackgroundJobClient _backgroundJobClient;


	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="dictionaryRepository">Implementation of <seealso cref="IDictionaryRepository"/>.</param>
	/// <param name="languageRepository">Implementation of <seealso cref="ILanguageRepository"/>.</param>
	/// <param name="backgroundJobClient"></param>
	public DictionariesController(IDictionaryRepository dictionaryRepository,
		ILanguageRepository languageRepository,
		IBackgroundJobClient backgroundJobClient)
	{
		_dictionaryRepository = dictionaryRepository;
		_languageRepository = languageRepository;
		_backgroundJobClient = backgroundJobClient;
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
		
		DateTime start = DateTime.Now;
		
		IEnumerable<Dictionary> dictionaries = (await _dictionaryRepository.GetDictionariesAsync(id,ietfLanguageTag)).ToArray();

		List<Link> links = new List<Link>();
		links.Add(new Link()
		{
			Action = "get",
			Rel = "self",
			Types = new[] { JSON_MIME_TYPE },
			HRef = BuildPageNavigationUrl(id,ietfLanguageTag, offsetIndex, pageSize)
		});
		
		if (offsetIndex > 0)
		{
			if (offsetIndex - pageSize < 0)
			{
				// previous page, but offset was incorrect
				links.Add(new Link()
				{
					Action = "get",
					Rel = "previouspage",
					HRef = BuildPageNavigationUrl(id,ietfLanguageTag, 0, pageSize),
					Types = new[] { JSON_MIME_TYPE }
				});
			}
			else
			{
				links.Add(new Link()
				{
					Action = "get",
					Rel = "previouspage",
					HRef = BuildPageNavigationUrl(id,ietfLanguageTag, offsetIndex-pageSize, pageSize),
					Types = new [] { JSON_MIME_TYPE }
				});
			}

		}

		decimal numberOfPages = Math.Ceiling(dictionaries.Count()/(decimal)pageSize);
		decimal offsetIndexOfLastPage = numberOfPages * pageSize;
		
		if (offsetIndexOfLastPage <= dictionaries.Count())
		{
			links.Add(new Link()
			{
				Action = "get",
				Rel = "nextpage",
				HRef = BuildPageNavigationUrl(id,ietfLanguageTag, offsetIndex+pageSize, pageSize),
				Types = new [] { JSON_MIME_TYPE }
			});
		}
		
		GetDictionariesResult getDictionariesResult = new GetDictionariesResult()
		{
			Results = dictionaries.Skip(offsetIndex).Take(pageSize).Select(d => new GetDictionaryResultItem()
			{
				Id = d.Id,
				Description = d.Description,
				Name = d.Name,
				CreatedAt = d.CreatedAt,
				CreatedOn = d.CreatedOn,
				SourceUrl = d.SourceUrl,
				IetfLanguageTag = d.IetfLanguageTag,
				CreatedByUserName = d.CreatedByUserName,
				Controller = d.Controller,
				ContentTypeKey = d.ContentTypeKey,
				ContentTypeFriendlyName = d.ContentTypeFriendlyName,
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
						Rel = "firstcontent",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/{d.Controller ?? "null"}?dictionaryId={d.Id}"
					}
				}
			}),
			Links = links.ToArray(),
			FromIndex = offsetIndex,
			PageSize = pageSize,
			TotalItemsCount = dictionaries.Count(),
			DeltaMs = (DateTime.Now-start).TotalMilliseconds
		};
		return getDictionariesResult;
	}

	private string BuildPageNavigationUrl(int? id, string? ietfLanguageTag, int offsetIndex, int pageSize)
	{
		StringBuilder sb = new StringBuilder(GetBaseApiPath()+"/dictionaries?");
		if (id.HasValue) sb.Append($"id={id.Value}&");
		if (!string.IsNullOrWhiteSpace(ietfLanguageTag)) sb.Append($"ietfLanguageTag={UrlEncoder.Default.Encode(ietfLanguageTag)}&");
		sb.Append($"offsetIndex={offsetIndex}");
		if (pageSize != Defaults.MaxItems) sb.Append($"&pageSize={pageSize}");
		return sb.ToString();
		
	}


	/// <summary>
	/// Creates a new Dictionary.
	/// </summary>
	/// <param name="createDictionary">A <see cref="Taggloo4.Dto.CreateDictionary"/> representing the Dictionary to create.</param>
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
			SourceUrl = createDictionary.SourceUrl,
			Controller = createDictionary.Controller,
			ContentTypeKey = createDictionary.ContentTypeKey,
			ContentTypeFriendlyName = createDictionary.ContentTypeFriendlyName
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

	

	/// <summary>
	/// Deletes a Dictionary and all associated content
	/// </summary>
	/// <param name="id">ID of the Dictionary to delete.</param>
	/// <returns>A <seealso cref="DeleteDictionaryResult"/> with status and detail on interrogating progress.</returns>
	/// <response code="202">Dictionary deletion has been requested.</response>
	/// <response code="400">One or more validation errors prevented successful deletion.</response>
	/// <response code="403">Not permitted.</response>
	/// <response code="404">Requested Dictionary is not found.</response>
	[HttpDelete("{id}")]
	[Authorize(Roles="administrator")]
	public async Task<ActionResult> DeleteDictionary(int id)
	{
		Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(id);
		if (dictionary == null) return NotFound();
		
		_backgroundJobClient.Enqueue<DeleteDictionaryJob>(job =>
			job.DeleteDictionary(id)
		);

		DeleteDictionaryResult result = new DeleteDictionaryResult()
		{
			DictionaryId = id,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/dictionaries/{dictionary.Id}",
					Types = new[]
					{
						JSON_MIME_TYPE
					}
				}
			}
		};

		return Accepted(result);
		
	}

	
}