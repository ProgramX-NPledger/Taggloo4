using System.ComponentModel.DataAnnotations;

namespace API.Model;

public class Dictionary
{
	public int Id { get; set; }
	
	[Required] 
	public string Name { get; set; }

	[MaxLength(1024)]
	public string Description { get; set; }

	public string SourceUrl { get; set; }

	public string IetfLanguageTag { get; set; }
	
	public required string CreatedByUserName { get; set; }

	public required DateTime CreatedAt { get; set; }

	public required string CreatedOn { get; set; }

	public Language Language { get; set; }

	public ICollection<Word> Words { get; set; }

	public ICollection<WordTranslation> WordTranslations { get; set; }
	
	
}