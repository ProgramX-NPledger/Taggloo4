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
	public required string Name { get; set; }

	/// <summary>
	/// Description of the Dictionary.
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// URL of the source of the Dictionary.
	/// </summary>
	public required string SourceUrl { get; set; }

	/// <summary>
	/// IETF Language Tag for the Dictionary.
	/// </summary>
	public required string IetfLanguageTag { get; set; }
	
	/// <summary>
	/// The URL name of the Controller to use to retrieve content.
	/// </summary>
	[MaxLength(32)]
	public string? Controller { get; set; }

	/// <summary>
	/// Disambiguated identifier for type of content to allow automatic processing.
	/// </summary>
	[MaxLength(32)]
	public string? ContentTypeKey { get; set; }

	/// <summary>
	/// Human-suitable name of Content Type.
	/// </summary>
	[MaxLength(128)]
	public string? ContentTypeFriendlyName { get; set; }
	
}