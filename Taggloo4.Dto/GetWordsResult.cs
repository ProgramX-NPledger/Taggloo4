namespace Taggloo4.Dto;

public class GetWordsResult
{
	public IEnumerable<GetWordResultItem> Results { get; set; } = Enumerable.Empty<GetWordResultItem>();
	public int FromIndex { get; set; }
	public int TotalItemsCount { get; set; }
	public int PageSize { get; set; }
	public IEnumerable<Link>? Links { get; set; }
	
}