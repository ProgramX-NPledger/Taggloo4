namespace Taggloo4Mgt.Importing.Model;

public class WordTranslation
{
	public int Id { get; set; }
	public required string TheTranslation { get; set; }
	public required string LanguageCode { get; set; }
	public DateTime CreatedAt { get; set; }
	public required string CreatedByUserName { get; set; }

	public int FromWordId { get; set; }

	public required string FromWord { get; set; }
}