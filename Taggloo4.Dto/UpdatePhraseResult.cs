
using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a Phrase update request.
/// </summary>
public class UpdatePhraseResult
{

	/// <summary>
	/// When <c>True</c> indicates that the Dictionary that this Phrase belongs to after the Update must be reindexed.
	/// </summary>
	public bool RequiresDictionaryReindexing { get; set; }

	
	/// <summary>
	/// Internal identification of Phrase
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link>? Links { get; set; }
	
}