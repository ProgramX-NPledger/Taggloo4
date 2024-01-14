using System.Text.Json.Serialization;

namespace API.DTO;

/// <summary>
/// Result of a Word creation request.
/// </summary>
public class CreateWordResult
{
	/// <summary>
	/// Internal identification of Word
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link> Links { get; set; }
	
}