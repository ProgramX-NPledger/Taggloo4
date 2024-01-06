using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Model;

public class SiteConfiguration
{
	public int Id { get; set; }
	
	[Required]
	public string Name { get; set; }

	
	
	
}