using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Communty Content Collections.
/// </summary>
public class CommunityContentCollectionRepository : RepositoryBase<CommunityContentCollection>, ICommunityContentCollectionRepository
{
	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public CommunityContentCollectionRepository(DataContext dataContext) : base(dataContext)
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


	/// <inheritdoc cref="ICommunityContentCollectionRepository.GetCommunityContentCollectionsAsync"/>
	public async Task<IEnumerable<CommunityContentCollection>> GetCommunityContentCollectionsAsync(string? containingText)
	{
		IQueryable<CommunityContentCollection> query = DataContext.CommunityContentCollections
			.Include(m=>m.CommunityContentDiscoverer)
			.AsQueryable();
		
		if (!string.IsNullOrWhiteSpace(containingText))
		{
			query = query.Where(q => q.Name.Contains(containingText));
		}
		
		return await query.ToArrayAsync();
		
	}

	/// <inheritdoc cref="ICommunityContentCollectionRepository.GetByIdAsync"/>
	public async Task<CommunityContentCollection?> GetByIdAsync(int id)
	{
		return await DataContext.CommunityContentCollections
			.Include(m=>m.CommunityContentDiscoverer)
			.SingleOrDefaultAsync(q => q.Id == id);
	}

	/// <inheritdoc cref="ICommunityContentItemRepository.CreateCommunityContentCollection"/>
	public CommunityContentCollection CreateCommunityContentCollection(string name, string? searchUrl, string? discovererKey = null)
	{
		// try and get a Discoverer by name
		discovererKey = discovererKey ?? $"Imported/{name}";
		
		CommunityContentDiscoverer? communityContentDiscoverer = DataContext.CommunityContentDiscoverers.SingleOrDefault(c => c.Key == discovererKey);
		
		// if new, create one, but without .NET reflected types
		if (communityContentDiscoverer == null)
		{
			communityContentDiscoverer = new CommunityContentDiscoverer()
			{
				Key = discovererKey,
				Name = name,
				Description = $"Created during import for Collection '{name}'"
			};
			DataContext.CommunityContentDiscoverers.Add(communityContentDiscoverer);
		}
		
		CommunityContentCollection communityContentCollection = new CommunityContentCollection()
		{
			Name = name,
			SearchUrl = searchUrl,
			CommunityContentDiscoverer = communityContentDiscoverer, 
			IsPollingEnabled = false,
			LastPolledAt = null
		};
		DataContext.CommunityContentCollections.Add(communityContentCollection);
		return communityContentCollection;
	}


	
}