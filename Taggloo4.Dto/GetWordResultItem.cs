namespace Taggloo4.Dto;

public class GetWordResultItem
{
	public int Id { get; set; }
	public string Word { get; set; }
	public IEnumerable<Link> Links { get; set; }
	
}