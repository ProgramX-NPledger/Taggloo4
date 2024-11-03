using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore.Identity;
using Taggloo4.Dto;
using Taggloo4.Model;
using Taggloo4.Web.Controllers;

namespace Taggloo4.Web.Areas.Api.Controllers;

/// <summary>
/// Community Content Item operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommunityContentItemsController : BaseApiController
{
	private readonly ICommunityContentItemRepository _communityContentItemRepository;
	private readonly IDictionaryRepository _dictionaryRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="communityContentItemRepository">Implementation of <seealso cref="ICommunityContentItemRepository"/></param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	public CommunityContentItemsController(ICommunityContentItemRepository communityContentItemRepository,
		IDictionaryRepository dictionaryRepository)
	{
		_communityContentItemRepository = communityContentItemRepository;
		_dictionaryRepository = dictionaryRepository;
	}
	
	
	/// <summary>
	/// Return a Community Content Item by ID.
	/// </summary>
	/// <param name="id">ID of the Community Content Item to return.</param>
	/// <response code="200">Community Content Item returned.</response>
	/// <response code="404">Community Content Item not found.</response>
	[HttpGet("{id}")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetCommunityContentItemResultItem>> GetCommunityContentItem(int id)
	{
		CommunityContentItem? communityContentItem = await _communityContentItemRepository.GetByIdAsync(id);
		if (communityContentItem == null) return NotFound();

		return Ok(new GetCommunityContentItemResultItem()
		{
			Hash = communityContentItem.Hash,
			Title = communityContentItem.Title,
			AuthorName = communityContentItem.AuthorName,
			AuthorUrl = communityContentItem.AuthorUrl,
			HashAlgorithm = communityContentItem.HashAlgorithm,
			ImageUrl = communityContentItem.ImageUrl,
			SynopsisText = communityContentItem.SynopsisText,
			OriginalSynopsisHtml = communityContentItem.OriginalSynopsisHtml,
			DictionaryId = communityContentItem.DictionaryId,
			Id = communityContentItem.Id,
			IsTruncated = communityContentItem.IsTruncated,
			PublishedAt = communityContentItem.PublishedAt,
			CommunityContentCollectionId = communityContentItem.CommunityContentCollectionId,
			RetrievedAt = communityContentItem.RetrievedAt,
			IetfLanguageTag = communityContentItem.Dictionary?.IetfLanguageTag ?? string.Empty,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/communitycontentitems/{communityContentItem.Id}",
					Types = new[] { JSON_MIME_TYPE }
				},
				new Link()
				{
					Action = "get",
					Rel = "dictionary",
					HRef = $"{GetBaseApiPath()}/dictionaries/{communityContentItem.DictionaryId}",
					Types = new[] { JSON_MIME_TYPE }
				},
				new Link()
				{
					Action = "get",
					Rel = "collection",
					HRef = $"{GetBaseApiPath()}/communitycontentcollections/{communityContentItem.CommunityContentCollectionId}",
					Types = new[] { JSON_MIME_TYPE }
				}
			}
		});
	}


	/// <summary>
	/// Retrieve matching Community Content Items.
	/// </summary>
	/// <param name="containingText">If specified, filters by the provided text.</param>
	/// <param name="dictionaryId">If specified, searches within the Dictionary represented by the ID.</param>
	/// <param name="hashAlgorithm">If specified, filters by the hash string algorithm.</param>
	/// <param name="ietfLanguageTag">If specified, filters within Dictionaries with the IETF Language Code.</param>
	/// <param name="offsetIndex">If specified, returns results starting at the specified offset position (starting index 0) Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <param name="pageSize">If specified, limits the number of results to the specified limit. Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <param name="hash">If specified, filters by the hash.</param>
	/// <response code="200">Results prepared.</response>
	/// <response code="403">Not permitted.</response>
	[HttpGet()]
	[Authorize(Roles="administrator, dataExporter")]
	public async Task<ActionResult<GetCommunityContentItemsResult>> GetCommunityContentItems(
		string? containingText,
		int? dictionaryId,
		string? hash,
		string? hashAlgorithm,
		string? ietfLanguageTag,
		int offsetIndex=Defaults.OffsetIndex, 
		int pageSize = Defaults.MaxItems)
	{
		AssertApiConstraints(pageSize);
		
		DateTime start = DateTime.Now;
		
		IEnumerable<CommunityContentItem> items = (await _communityContentItemRepository.GetCommunityContentItemsAsync(dictionaryId, containingText, hash, hashAlgorithm, null,ietfLanguageTag)).ToArray();

		List<Link> links = new List<Link>();
		links.Add(new Link()
		{
			Action = "get",
			Rel = "self",
			Types = new[] { JSON_MIME_TYPE },
			HRef = BuildPageNavigationUrl(containingText,dictionaryId, hash, hashAlgorithm, ietfLanguageTag,offsetIndex, pageSize)
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
					HRef = BuildPageNavigationUrl(containingText, dictionaryId, hash, hashAlgorithm, ietfLanguageTag,0, pageSize),
					Types = new[] { JSON_MIME_TYPE }
				});
			}
			else
			{
				links.Add(new Link()
				{
					Action = "get",
					Rel = "previouspage",
					HRef = BuildPageNavigationUrl(containingText, dictionaryId, hash, hashAlgorithm, ietfLanguageTag,offsetIndex - pageSize, pageSize),
					Types = new [] { JSON_MIME_TYPE }
				});
			}

		}
		
		decimal numberOfPages = Math.Ceiling(items.Count()/(decimal)pageSize);
		decimal offsetIndexOfLastPage = numberOfPages * pageSize;
		decimal remainder = numberOfPages % pageSize;
		offsetIndexOfLastPage -= remainder;

		if (offsetIndexOfLastPage <= items.Count())
		{
			links.Add(new Link()
			{
				Action = "get",
				Rel = "nextpage",
				HRef = BuildPageNavigationUrl(containingText, dictionaryId, hash,hashAlgorithm, ietfLanguageTag, offsetIndex + pageSize, pageSize),
				Types = new [] { JSON_MIME_TYPE }
			});
		}
		
		GetCommunityContentItemsResult getCommunityContentItemsResult = new GetCommunityContentItemsResult()
		{
			Results = items.Skip(offsetIndex).Take(pageSize).Select(c => new GetCommunityContentItemResultItem()
			{
				Id = c.Id,
				Hash = c.Hash,
				Title = c.Title,
				AuthorName = c.AuthorName,
				AuthorUrl = c.AuthorUrl,
				HashAlgorithm = c.HashAlgorithm,
				ImageUrl = c.ImageUrl,
				SynopsisText = c.SynopsisText,
				IetfLanguageTag = c.Dictionary?.IetfLanguageTag ?? string.Empty,
				OriginalSynopsisHtml = c.OriginalSynopsisHtml,
				DictionaryId = c.DictionaryId,
				IsTruncated = c.IsTruncated,
				PublishedAt = c.PublishedAt,
				CommunityContentCollectionId = c.CommunityContentCollectionId,
				RetrievedAt = c.RetrievedAt,
				Links = new[]
				{
					new Link()
					{
						Action = "get",
						Rel = "self",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/communitycontentitems/{c.Id}"
					},
					new Link()
					{
						Action = "get",
						Rel = "dictionary",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/dictionaries/{c.DictionaryId}"
					},
					new Link()
					{
						Action = "get",
						Rel = "collection",
						HRef = $"{GetBaseApiPath()}/communitycontentcollections/{c.CommunityContentCollectionId}",
						Types = new[] { JSON_MIME_TYPE }
					}
				}
			}),
			Links = links.ToArray(),
			FromIndex = offsetIndex,
			PageSize = pageSize,
			TotalItemsCount = items.Count(),
			DeltaMs = (DateTime.Now-start).TotalMilliseconds
		};
		return getCommunityContentItemsResult;
	}
	
	private string BuildPageNavigationUrl(string? containingText, int? dictionaryId, string? hash, string? hashAlgorithm, string? ietfLanguageTag,  int offsetIndex, int pageSize)
	{
		StringBuilder sb = new StringBuilder(GetBaseApiPath()+"/communitycontentitems?");
		if (!string.IsNullOrWhiteSpace(containingText)) sb.Append($"containingText={UrlEncoder.Default.Encode(containingText)}&");
		if (dictionaryId.HasValue) sb.Append($"dictionaryId={dictionaryId}&");
		if (!string.IsNullOrWhiteSpace(hash)) sb.Append($"hash={UrlEncoder.Default.Encode(hash)}&");
		if (!string.IsNullOrWhiteSpace(hashAlgorithm)) sb.Append($"hashAlgorithm={UrlEncoder.Default.Encode(hashAlgorithm)}&");
		if (!string.IsNullOrWhiteSpace(ietfLanguageTag)) sb.Append($"ietfLanguageTag={UrlEncoder.Default.Encode(ietfLanguageTag)}&");
		sb.Append($"offsetIndex={offsetIndex}&");
		if (pageSize != Defaults.MaxItems) sb.Append($"pageSize={pageSize}");
		return sb.ToString();
	}
	
    /// <summary>
	/// Creates a new Community Content Item.
	/// </summary>
	/// <param name="createCommunityContentItem">A <see cref="CreateCommunityContentItem"/> representing the Community Content Item to create.</param>
	/// <returns>The created Community Content Item.</returns>
	/// <response code="201">Community Content Item was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost]
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<CreateCommunityContentItemResult>> CreateCommunityContentItem(CreateCommunityContentItem createCommunityContentItem)
	{
		// try to resolve the dictionary
		Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(createCommunityContentItem.DictionaryId);
		if (dictionary == null) return BadRequest("Invalid Dictionary");
		
		// calculate hash of item
		byte[] dataToHash = Encoding.ASCII.GetBytes(createCommunityContentItem.OriginalSynopsisHtml);
		byte[] md5Hashed = MD5.HashData(dataToHash);
		StringBuilder md5HashStringBuilder = new StringBuilder(md5Hashed.Length);
		for (int i=0;i < md5Hashed.Length; i++)
		{
			md5HashStringBuilder.Append(md5Hashed[i].ToString("X2"));
		}
		string md5HashString = md5HashStringBuilder.ToString();
		
		// detect if same hash already exists
		IEnumerable<CommunityContentItem> matchingCommunityContentItems =
			await _communityContentItemRepository.GetCommunityContentItemsAsync(createCommunityContentItem.DictionaryId,
				null, md5HashString, nameof(MD5), null,null);
		
		// if already exists, return 400
		if (matchingCommunityContentItems.Any())
		{
			return BadRequest("Existing Community Content Item with same Hash exists in same Dictionary");
		}
		
		// get collection by name
		bool createdNewCollection = false;
		string collectionName = createCommunityContentItem.CollectionName;
		IEnumerable<CommunityContentCollection> matchingCollections =
			(await _communityContentItemRepository.GetCommunityContentCollectionsAsync(
				collectionName, null)).ToArray();
		CommunityContentCollection communityContentCollection;
		if (!matchingCollections.Any())
		{
			// is a new collection, so create
			communityContentCollection =
				_communityContentItemRepository.CreateCommunityContentCollection(createCommunityContentItem
					.CollectionName,null);
			createdNewCollection = true;
		}
		else
		{
			if (matchingCollections.Count() > 1)
			{
				return BadRequest("More than one Community Content Collection matching collection name");
			}
			communityContentCollection = matchingCollections.Single();
		}
		
		// create
		CommunityContentItem communityContentItem =
			_communityContentItemRepository.CreateCommunityContentItem(
				createCommunityContentItem.Title,
				createCommunityContentItem.SourceUrl,
				createCommunityContentItem.AuthorName,
				createCommunityContentItem.AuthorUrl,
				createCommunityContentItem.ImageUrl,
				createCommunityContentItem.SynopsisText,
				createCommunityContentItem.OriginalSynopsisHtml,
				createCommunityContentItem.ExternalId,
				createCommunityContentItem.CreatedAt,
				createCommunityContentItem.CreatedOn,
				createCommunityContentItem.CreatedByUserName,
				createCommunityContentItem.RetrievedAt,
				createCommunityContentItem.PublishedAt,
				createCommunityContentItem.IsTruncated,
				communityContentCollection,
				dictionary
			);

		if (!await _communityContentItemRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/communityContentItems/{communityContentItem.Id}";
		return Created(url,new CreateCommunityContentItemResult()
		{
			ExternalId = communityContentItem.ExternalId,
			CommunityContentItemId = communityContentItem.Id,
			CommunityContentCollectionId = communityContentCollection.Id,
			NewCollectionCreated = createdNewCollection,
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
	/// Update an existing Community Content Item with meta-data.
	/// </summary>
	/// <param name="id">Identifier of Community Content Item to update.</param>
	/// <param name="updateCommunityContentItem">A <see cref="UpdatePhrase"/> representing the Phrase to update.</param>
	/// <returns>The updated Phrase.</returns>
	/// <response code="200">Phrase was updated.</response>
	/// <response code="400">One or more validation errors prevented successful updating.</response>
	/// <response code="403">Not permitted.</response>
	/// <response code="404">Phrase not found.</response>
	[HttpPatch("{id:int}")]
	[Authorize(Roles="administrator, dataImporter")]
	public async Task<ActionResult<UpdateCommunityContentItemResult>> UpdateCommunityContentItem(int id, UpdateCommunityContentItem updateCommunityContentItem)
	{
		CommunityContentItem? communityContentItem = await _communityContentItemRepository.GetByIdAsync(id);
		if (communityContentItem == null) return NotFound();
		
		// update community content item
		UpdateCommunityContentItemResult updateCommunityContentItemResult = new UpdateCommunityContentItemResult()
		{

		};
		
		if (updateCommunityContentItem.CreatedOn != null) communityContentItem.CreatedOn = updateCommunityContentItem.CreatedOn;
		if (updateCommunityContentItem.CreatedByUserName != null) communityContentItem.CreatedByUserName = updateCommunityContentItem.CreatedByUserName;
		if (updateCommunityContentItem.CreatedAt.HasValue) communityContentItem.CreatedAt = updateCommunityContentItem.CreatedAt.Value;
		
		_communityContentItemRepository.Update(communityContentItem);
		if (!await _communityContentItemRepository.SaveAllAsync()) return BadRequest();
		
		string url = $"{GetBaseApiPath()}/communityContentItems/{communityContentItem.Id}";
		updateCommunityContentItemResult.Id = communityContentItem.Id;
		updateCommunityContentItemResult.Links = new[]
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
				HRef = $"{GetBaseApiPath()}/dictionaries/{communityContentItem.DictionaryId}"
			}
		};
		
		return Ok(updateCommunityContentItemResult);
		
		
	}

	
}