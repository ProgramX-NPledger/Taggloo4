using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Language
/// </summary>
public class GetLanguageResult
{
	/// <summary>
	/// IETF Tag used to represent the Language.
	/// </summary>
	public required string IetfLanguageCode { get; set; }

	/// <summary>
	/// Name of the Language
	/// </summary>
	public required string Name { get; set; }
	
	/// <summary>
	/// A list of Links referring to related Entities.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
	
}