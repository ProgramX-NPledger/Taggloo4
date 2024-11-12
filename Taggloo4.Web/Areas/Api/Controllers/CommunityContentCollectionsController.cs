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
using CommunityContentCollection = Taggloo4.Model.CommunityContentCollection;
using CommunityContentDiscoverer = Taggloo4.Dto.CommunityContentDiscoverer;

namespace Taggloo4.Web.Areas.Api.Controllers;

/// <summary>
/// Community Content Collection operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommunityContentCollectionsController : BaseApiController
{
	private readonly ICommunityContentCollectionRepository _communityContentCollectionRepository;
	private readonly IDictionaryRepository _dictionaryRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="communityContentCollectionRepository">Implementation of <seealso cref="ICommunityContentCollectionRepository"/></param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	public CommunityContentCollectionsController(ICommunityContentCollectionRepository communityContentCollectionRepository,
		IDictionaryRepository dictionaryRepository)
	{
		_communityContentCollectionRepository = communityContentCollectionRepository;
		_dictionaryRepository = dictionaryRepository;
	}
	
	
	/// <summary>
	/// Return a Community Content Collection by ID.
	/// </summary>
	/// <param name="id">ID of the Community Content Collection to return.</param>
	/// <response code="200">Community Content Collection returned.</response>
	/// <response code="404">Community Content Collection not found.</response>
	[HttpGet("{id}")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetCommunityContentCollectionResultItem>> GetCommunityContentCollection(int id)
	{
		CommunityContentCollection? communityContentCollection = await _communityContentCollectionRepository.GetByIdAsync(id);
		if (communityContentCollection == null) return NotFound();

		// TODO: Maybe include number of items within
		
		return Ok(new GetCommunityContentCollectionResultItem()
		{
			Name = communityContentCollection.Name,
			SearchUrl = communityContentCollection.SearchUrl,
			IsPollingEnabled = communityContentCollection.IsPollingEnabled,
			LastPolledAt = communityContentCollection.LastPolledAt,
			Id = communityContentCollection.Id,
			PollFrequencyMins = communityContentCollection.PollFrequencyMins,
			Discoverer = new CommunityContentDiscoverer()
			{
				Key	= communityContentCollection.CommunityContentDiscoverer!.Key,
				Name = communityContentCollection.CommunityContentDiscoverer!.Name,
				Id = communityContentCollection.CommunityContentDiscoverer!.Id,
				CommunityContentDiscovererDotNetAssemblyName = communityContentCollection.CommunityContentDiscoverer!.CommunityContentDiscovererDotNetAssemblyName,
				CommunityContentDiscovererDotNetTypeName = communityContentCollection.CommunityContentDiscoverer!.CommunityContentDiscovererDotNetTypeName
			},
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/communitycontentcollections/{communityContentCollection.Id}",
					Types = new[] { JSON_MIME_TYPE }
				}
			}
		});
	}


	/// <summary>
	/// Retrieve matching Community Content Collections.
	/// </summary>
	/// <param name="containingText">If specified, filters by the provided text.</param>
	/// <param name="offsetIndex">If specified, returns results starting at the specified offset position (starting index 0) Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <param name="pageSize">If specified, limits the number of results to the specified limit. Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <response code="200">Results prepared.</response>
	/// <response code="403">Not permitted.</response>
	[HttpGet()]
	[Authorize(Roles="administrator, dataExporter")]
	public async Task<ActionResult<GetCommunityContentCollectionsResult>> GetCommunityContentItems(
		string? containingText,
		int offsetIndex=Defaults.OffsetIndex, 
		int pageSize = Defaults.MaxItems)
	{
		AssertApiConstraints(pageSize);
		
		DateTime start = DateTime.Now;
		
		IEnumerable<CommunityContentCollection> items = (await _communityContentCollectionRepository.GetCommunityContentCollectionsAsync(containingText)).ToArray();

		List<Link> links = new List<Link>();
		links.Add(new Link()
		{
			Action = "get",
			Rel = "self",
			Types = new[] { JSON_MIME_TYPE },
			HRef = BuildPageNavigationUrl(containingText,offsetIndex, pageSize)
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
					HRef = BuildPageNavigationUrl(containingText,0, pageSize),
					Types = new[] { JSON_MIME_TYPE }
				});
			}
			else
			{
				links.Add(new Link()
				{
					Action = "get",
					Rel = "previouspage",
					HRef = BuildPageNavigationUrl(containingText, offsetIndex - pageSize, pageSize),
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
				HRef = BuildPageNavigationUrl(containingText,  offsetIndex + pageSize, pageSize),
				Types = new [] { JSON_MIME_TYPE }
			});
		}
		
		GetCommunityContentCollectionsResult getCommunityContentCollectionsResult = new GetCommunityContentCollectionsResult()
		{
			Results = items.Skip(offsetIndex).Take(pageSize).Select(communityContentCollection => new GetCommunityContentCollectionResultItem()
			{
				Name = communityContentCollection.Name,
				SearchUrl = communityContentCollection.SearchUrl,
				IsPollingEnabled = communityContentCollection.IsPollingEnabled,
				LastPolledAt = communityContentCollection.LastPolledAt,
				Id = communityContentCollection.Id,
				PollFrequencyMins = communityContentCollection.PollFrequencyMins,
				Discoverer = new CommunityContentDiscoverer()
				{
					Key	= communityContentCollection.CommunityContentDiscoverer!.Key,
					Name = communityContentCollection.CommunityContentDiscoverer!.Name,
					Id = communityContentCollection.CommunityContentDiscoverer!.Id,
					CommunityContentDiscovererDotNetAssemblyName = communityContentCollection.CommunityContentDiscoverer!.CommunityContentDiscovererDotNetAssemblyName,
					CommunityContentDiscovererDotNetTypeName = communityContentCollection.CommunityContentDiscoverer!.CommunityContentDiscovererDotNetTypeName
				},
				Links = new[]
				{
					new Link()
					{
						Action = "get",
						Rel = "self",
						HRef = $"{GetBaseApiPath()}/communitycontentcollections/{communityContentCollection.Id}",
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
		return getCommunityContentCollectionsResult;
	}
	
	private string BuildPageNavigationUrl(string? containingText,  int offsetIndex, int pageSize)
	{
		StringBuilder sb = new StringBuilder(GetBaseApiPath()+"/communitycontentcollections?");
		if (!string.IsNullOrWhiteSpace(containingText)) sb.Append($"containingText={UrlEncoder.Default.Encode(containingText)}&");
		sb.Append($"offsetIndex={offsetIndex}&");
		if (pageSize != Defaults.MaxItems) sb.Append($"pageSize={pageSize}");
		return sb.ToString();
	}
	

	
}