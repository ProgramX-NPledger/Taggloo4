using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a Dictionary creation request.
/// </summary>
public class CreateDictionaryResult
{
	/// <summary>
	/// The ID of the Dictionary.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link>? Links { get; set; }
	
}