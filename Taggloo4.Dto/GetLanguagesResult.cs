namespace Taggloo4.Dto;

public class GetLanguagesResult
{
	public IEnumerable<GetLanguageResultItem> Results { get; set; } = Enumerable.Empty<GetLanguageResultItem>();
	public int FromIndex { get; set; }
	public int TotalItemsCount { get; set; }
	public int PageSize { get; set; }
	public IEnumerable<Link>? Links { get; set; }
	
}