using Taggloo4.Model;

namespace Taggloo4.Contract;

/// <summary>
/// Represents an abstraction for working with Community Content Items.
/// </summary>
public interface ICommunityContentItemRepository : IRepositoryBase<CommunityContentItem>
{


	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();

	/// <summary>
	/// Retrieves all matching <seealso cref="CommunityContentItem"/>s.
	/// </summary>
	/// <param name="dictionaryId">The ID of the <seealso cref="Dictionary"/> to search.</param>
	/// <param name="containingText">Filters response for presence of text (collation as per database).</param>
	/// <param name="hashAlgorithm">If specified, filters for the specific hash algorithm.</param>
	/// <param name="externalId">If specified, filters on matching the external identifier.</param>
	/// <param name="languageCode">If specified, filters within Dictionaries for the IETF Language Tag.</param>
	/// <param name="hash">If specified, filters for the specific hash.</param>
	/// <returns>A collection of matching <seealso cref="CommunityContentItem"/>s.</returns>
	Task<IEnumerable<CommunityContentItem>> GetCommunityContentItemsAsync(int? dictionaryId, string? containingText, string? hash, string? hashAlgorithm, string? externalId, string? languageCode);
	
	/// <summary>
	/// Retrieves a <seealso cref="CommunityContentItem"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="CommunityContentItem"/>.</param>
	/// <returns>The requested <seealso cref="CommunityContentItem"/>, or <c>null</c> if no CommunityContentItem could be found./</returns>
	Task<CommunityContentItem?> GetByIdAsync(int id);

	/// <summary>
	/// Retrieves all matching <seealso cref="CommunityContentCollection"/>s.
	/// </summary>
	/// <param name="containingText">If specified, filters on presence of text in Name (collation as per database).</param>
	/// <param name="id">If specified, filters on identifier.</param>
	/// <returns>A collection of matching <seealso cref="CommunityContentCollection"/>s.</returns>
	Task<IEnumerable<CommunityContentCollection>> GetCommunityContentCollectionsAsync(
		string? containingText, int? id);


	/// <summary>
	/// Create a Communitu Contact Item and schedule for addition.
	/// </summary>
	/// <param name="title">The title of the item.</param>
	/// <param name="sourceUrl">The original URL of the item.</param>
	/// <param name="authorName">Author's name.</param>
	/// <param name="authorUrl">URL to Author's profile or feed.</param>
	/// <param name="imageUrl">URL of adjacent image.</param>
	/// <param name="synopsisText">Synopsis text (without HTML)</param>
	/// <param name="originalSynopsisHtml">Original synopsis text (may include HTML)</param>
	/// <param name="externalId">External reference for content.</param>
	/// <param name="createdAt">Time stamp of creation.</param>
	/// <param name="createdOn">Host/IP Address on which item was created.</param>
	/// <param name="createdByUserName">UserName of user creating the item.</param>
	/// <param name="retrievedAt">Time stamp of retrieval of item.</param>
	/// <param name="publishedAt">Time stamp of publication of item.</param>
	/// <param name="isTruncated">Whether the item was truncated at source.</param>
	/// <param name="addToCommunityContentCollection">Collection to which this item should be added.</param>
	/// <param name="addToDictionary">Dictionary to which this item should be added.</param>
	/// <returns>The created <seealso cref="CommunityContentItem"/>.</returns>
	/// <remarks>
	/// This method generates a hash which is applied to the item for rapid comparison.
	/// Hashing is performed on the <c>OriginalSynopsisHtml</c> field.
	/// </remarks>
	CommunityContentItem CreateCommunityContentItem(string title, string sourceUrl, string authorName, string authorUrl,
		string imageUrl,
		string synopsisText, string originalSynopsisHtml, string? externalId, DateTime createdAt, string createdOn,
		string createdByUserName, DateTime retrievedAt, DateTime publishedAt, bool isTruncated,
		CommunityContentCollection addToCommunityContentCollection, Dictionary addToDictionary);


}