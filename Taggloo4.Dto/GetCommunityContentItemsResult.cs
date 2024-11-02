namespace Taggloo4.Dto;

/// <summary>
/// Result of a request for many Community Content Items.
/// </summary>
public class GetCommunityContentItemsResult
{
	/// <summary>
	/// Results of the query.
	/// </summary>
	public IEnumerable<GetCommunityContentItemResultItem> Results { get; set; } = Enumerable.Empty<GetCommunityContentItemResultItem>();
	
	/// <summary>
	/// The index from which results have been returned. This is 0-based.
	/// </summary>
	public int FromIndex { get; set; }
	
	/// <summary>
	/// Total items that match the criteria, which may be greater than the number of returned results.
	/// </summary>
	public int TotalItemsCount { get; set; }
	
	/// <summary>
	/// Total number of items that could have been returned in this request.
	/// </summary>
	public int PageSize { get; set; }
	
	/// <summary>
	/// Links associated with this request.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
	/// <summary>
	/// Time for request in milliseconds.
	/// </summary>
	public double DeltaMs { get; set; }
}