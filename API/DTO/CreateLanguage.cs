using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.DTO;

/// <summary>
/// Represents a Language to be created using the POST Languages method.
/// </summary>
public class CreateLanguage
{
	/// <summary>
	/// The international code for the Language.
	/// This is a combination of ISO 639, ISO 15924m ISO 3166-1 anf UN M.49. 
	/// </summary>
	[Required]
	public required string IetfLanguageTag { get; set; }
	
	/// <summary>
	/// The friendly name of the Language.
	/// </summary>
	[Required]
	public required string Name { get; set; }
}