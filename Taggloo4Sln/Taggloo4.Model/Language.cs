using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Model;

public class Language
{
	public int Id { get; set; }
	
	[Required]
	public string Name { get; set; }

	[Required]
	public string Iso639Code { get; set; }
}