using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a Dictionary deletion request.
/// </summary>
public class DeleteDictionaryResult
{
	public int DictionaryId { get; set; }
	
	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link>? Links { get; set; }
	
}