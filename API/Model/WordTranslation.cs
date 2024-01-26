using System.ComponentModel.DataAnnotations;

namespace API.Model;

public class WordTranslation
{
	public int Id { get; set; }
	public int FromWordId { get; set; }
	// this breaks migrations
	//public Word FromWord { get; set; }
	public int ToWordId { get; set; }
	// this breaks migrations
	//public Word ToWord { get; set; }
	public string CreatedOn { get; set; }
	public string? CreatedByUserName { get; set; }
	public DateTime CreatedAt { get; set; }
	public int DictionaryId { get; set; }
	
	
	
	public Dictionary Dictionary { get; set; }
	
	
	
}