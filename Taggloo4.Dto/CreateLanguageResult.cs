using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a Language creation request.
/// </summary>
public class CreateLanguageResult
{
	/// <summary>
	/// The IETF Language Tag chosen for the user.
	/// </summary>
	public string IetfLanguageTag { get; set; }

	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link> Links { get; set; }
	
}