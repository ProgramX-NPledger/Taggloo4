using Taggloo4.Model;

namespace Taggloo4.Contract;

/// <summary>
/// Represents an abstraction for working with Community Content Collections.
/// </summary>
public interface ICommunityContentCollectionRepository : IRepositoryBase<CommunityContentCollection>
{


	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();

	/// <summary>
	/// Retrieves all matching <seealso cref="CommunityContentCollection"/>s.
	/// </summary>
	/// <param name="containingText">Filters response for presence of text (collation as per database).</param>
	/// <returns>A collection of matching <seealso cref="CommunityContentCollection"/>s.</returns>
	Task<IEnumerable<CommunityContentCollection>> GetCommunityContentCollectionsAsync(string? containingText);
	
	/// <summary>
	/// Retrieves a <seealso cref="CommunityContentCollection"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="CommunityContentCollection"/>.</param>
	/// <returns>The requested <seealso cref="CommunityContentCollection"/>, or <c>null</c> if no CommunityContentCollection could be found./</returns>
	Task<CommunityContentCollection?> GetByIdAsync(int id);

	/// <summary>
	/// Create a Community Content Collection and schedule for addition.
	/// </summary>
	/// <param name="name">Name of collection.</param>
	/// <param name="searchUrl">Search URL to apply to collection.</param>
	/// <param name="discovererKey">Key of Discoverer. If not specified, will create one using the <c>name</c> without a .NET specification.</param>
	/// <returns>The created <seealso cref="CommunityContentCollection"/>.</returns>
	CommunityContentCollection CreateCommunityContentCollection(string name, string? searchUrl,
		string? discovererKey = null);

}