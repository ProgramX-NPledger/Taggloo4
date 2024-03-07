using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a Word creation request.
/// </summary>
public class CreateWordResult
{
	/// <summary>
	/// A unique identifier that represents this creation operation.
	/// </summary>
	public Guid ImportId { get; set; }
	
	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link>? Links { get; set; }
	
}