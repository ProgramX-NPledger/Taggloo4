using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a Phrase creation request.
/// </summary>
public class CreatePhraseResult
{
	/// <summary>
	/// Externally determined identifier, or a GUID if not provided. This is saved alongside the Phrase to allow
	/// retrieval by a secondary key.
	/// </summary>
	public string? ExternalId { get; set; }

	/// <summary>
	/// Identifier of the Phrase.
	/// </summary>
	public int PhraseId { get; set; }
	
	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link>? Links { get; set; }
	
	/// <summary>
	/// When set indicates that the action will require a reindex operation.
	/// </summary>
	public bool RequiresReindexing { get; set; }
}