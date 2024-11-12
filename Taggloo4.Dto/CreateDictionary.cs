using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Dictionary to be created using the POST Dictonaries method.
/// </summary>
public class CreateDictionary
{
	/// <summary>
	/// Name of the Dictionary.
	/// </summary>
	[MaxLength(128)]
	public required string Name { get; set; }

	/// <summary>
	/// Description of the Dictionary.
	/// </summary>
	[MaxLength(1024)]
	public required string Description { get; set; }

	/// <summary>
	/// URL of the source of the Dictionary.
	/// </summary>
	[MaxLength(1024)]
	public required string SourceUrl { get; set; }

	/// <summary>
	/// IETF Language Tag for the Dictionary.
	/// </summary>
	[MaxLength(5)]
	public required string IetfLanguageTag { get; set; }
	
	/// <summary>
	/// Disambiguated identifier for type of content to allow automatic processing.
	/// </summary>
	[MaxLength(32)]
	public required string ContentTypeKey { get; set; }

	
}