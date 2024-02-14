using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a link towards a related entity.
/// </summary>
public class Link
{
	/// <summary>
	/// Entity relationship.
	/// </summary>
	[JsonPropertyName("rel")]
	public required string Rel { get; set; }

	/// <summary>
	/// HRef that may be used to extract the entity.
	/// </summary>
	[JsonPropertyName("href")]
	public required string HRef { get; set; }

	/// <summary>
	/// The Action (HTTP Verb) that is used when accessing the HRef.
	/// </summary>
	[JsonPropertyName("action")]
	public required string Action { get; set; }
	
	/// <summary>
	/// A collection of MIME types that may be returned.
	/// </summary>
	[JsonPropertyName("types")]
	public IEnumerable<string>? Types { get; set; }
		
	
}