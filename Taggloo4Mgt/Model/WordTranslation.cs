namespace Taggloo4Mgt.Model;

public class WordTranslation
{
	public required string TheTranslation { get; set; }
	public required string LanguageCode { get; set; }
	public DateTime CreatedAt { get; set; }
	public required string CreatedByUserName { get; set; }
	
}