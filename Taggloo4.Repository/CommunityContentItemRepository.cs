using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Phrases.
/// </summary>
public class CommunityContentItemRepository : RepositoryBase<CommunityContentItem>, ICommunityContentItemRepository
{
	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public CommunityContentItemRepository(DataContext dataContext) : base(dataContext)
	{
	}

	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	public async Task<bool> SaveAllAsync()
	{
		return await DataContext.SaveChangesAsync() > 0;
	}


	/// <inheritdoc cref="ICommunityContentItemRepository.GetCommunityContentItemsAsync"/>
	public async Task<IEnumerable<CommunityContentItem>> GetCommunityContentItemsAsync(int? dictionaryId, string? containingText, string? hash, string? hashAlgorithm, string? externalId, string? languageCode)
	{
		IQueryable<CommunityContentItem> query = DataContext.CommunityContentItems
			.Include("CommunityContentItems")
			.Include(m=>m.Dictionary).ThenInclude(m=>m.Language)
			.Include(m=>m.CommunityContentCollection).ThenInclude(m=>m.CommunityContentDiscoverer)
			.AsQueryable();
		
		if (dictionaryId.HasValue)
		{
			query = query.Where(q => q.DictionaryId == dictionaryId.Value);
		}

		if (!string.IsNullOrWhiteSpace(containingText))
		{
			query = query.Where(q => q.Title.Contains(containingText) || q.SynopsisText.Contains(containingText));
		}
		
		if (!string.IsNullOrWhiteSpace(externalId))
		{
			query = query.Where(q => q.ExternalId == externalId);
		}
		
		if (!string.IsNullOrWhiteSpace(hash))
		{
			query = query.Where(q => q.Hash == hash);
		}
		
		if (!string.IsNullOrWhiteSpace(hashAlgorithm))
		{
			query = query.Where(q => q.HashAlgorithm == hashAlgorithm);
		}

		if (!string.IsNullOrWhiteSpace(languageCode))
		{
			query = query.Where(q=>q.Dictionary != null && q.Dictionary.IetfLanguageTag==languageCode);
		}

		return await query.ToArrayAsync();
		
	}

	/// <inheritdoc cref="ICommunityContentItemRepository.GetByIdAsync"/>
	public async Task<CommunityContentItem?> GetByIdAsync(int id)
	{
		return await DataContext.CommunityContentItems
			.Include(m => m.Dictionary).ThenInclude(m => m.Language)
			.Include(m => m.CommunityContentCollection).ThenInclude(m => m.CommunityContentDiscoverer)
			.SingleOrDefaultAsync(q => q.Id == id);
	}

	/// <inheritdoc cref="ICommunityContentItemRepository.GetCommunityContentCollectionsAsync"/>
	public async Task<IEnumerable<CommunityContentCollection>> GetCommunityContentCollectionsAsync(string? containingText, int? id)
	{
		IQueryable<CommunityContentCollection> query = DataContext.CommunityContentCollections
			.Include(m=>m.CommunityContentDiscoverer)
			.AsQueryable();
		
		if (id.HasValue)
		{
			query = query.Where(q => q.Id == id.Value);
		}

		if (!string.IsNullOrWhiteSpace(containingText))
		{
			query = query.Where(q => q.Name.Contains(containingText));
		}
		
		return await query.ToArrayAsync();
	}

	/// <inheritdoc cref="ICommunityContentItemRepository.CreateCommunityContentCollection"/>
	public CommunityContentCollection CreateCommunityContentCollection(string name, string? searchUrl)
	{
		CommunityContentCollection communityContentCollection = new CommunityContentCollection()
		{
			Name = name,
			SearchUrl = searchUrl,
			IsPollingEnabled = false,
			LastPolledAt = null
		};
		DataContext.CommunityContentCollections.Add(communityContentCollection);
		return communityContentCollection;
	}

	/// <inheritdoc cref="ICommunityContentItemRepository.CreateCommunityContentItem"/>
	public CommunityContentItem CreateCommunityContentItem(string title, string sourceUrl, string authorName, string authorUrl, string imageUrl,
		string synopsisText, string originalSynopsisHtml, string? externalId, DateTime createdAt, string createdOn,
		string createdByUserName, DateTime retrievedAt, DateTime publishedAt, bool isTruncated,
		CommunityContentCollection addToCommunityContentCollection, Dictionary addToDictionary)
	{
		// calculate hash of item
		byte[] dataToHash = Encoding.ASCII.GetBytes(originalSynopsisHtml);
		byte[] md5Hashed = MD5.HashData(dataToHash);
		StringBuilder md5HashStringBuilder = new StringBuilder(md5Hashed.Length);
		for (int i=0;i < md5Hashed.Length; i++)
		{
			md5HashStringBuilder.Append(md5Hashed[i].ToString("X2"));
		}
		string md5HashString = md5HashStringBuilder.ToString();
		
		CommunityContentItem communityContentItem = new CommunityContentItem()
		{
			Title = title,
			SourceUrl = sourceUrl,
			Hash = md5HashString,
			AuthorName = authorName,
			AuthorUrl = authorUrl,
			HashAlgorithm = nameof(MD5),
			ImageUrl = imageUrl,
			SynopsisText = synopsisText,
			OriginalSynopsisHtml = originalSynopsisHtml,
			ExternalId = externalId,
			CommunityContentCollection = addToCommunityContentCollection,
			CreatedOn = createdOn,
			CreatedByUserName = createdByUserName,
			Dictionary = addToDictionary,
			CreatedAt = createdAt,
			IsTruncated = isTruncated,
			PublishedAt = publishedAt,
			RetrievedAt = retrievedAt
		};
		DataContext.CommunityContentItems.Add(communityContentItem);
		return communityContentItem;
	}

	
}