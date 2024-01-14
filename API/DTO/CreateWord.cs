using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.DTO;

/// <summary>
/// Represents a Word to be created using the POST Words method.
/// </summary>
public class CreateWord
{
	/// <summary>
	/// The international code for the Language of the Word.
	/// This is a combination of ISO 639, ISO 15924m ISO 3166-1 anf UN M.49. 
	/// </summary>
	[Required]
	public required string IetfLanguageTag { get; set; }
	
	/// <summary>
	/// The Word to create, in the language specified by <c>IetfLanguageTag</c>.
	/// </summary>
	[Required]
	public required string Word { get; set; }
}