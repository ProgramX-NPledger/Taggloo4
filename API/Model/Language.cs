using System.ComponentModel.DataAnnotations;

namespace API.Model;

public class Language
{
	[Required]
	[Key]
	[MaxLength(16)]
	public string IetfLanguageTag { get; set; }
	
	[Required] 
	public string Name { get; set; }
	
	
}