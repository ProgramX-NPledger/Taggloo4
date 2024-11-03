using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a Phrase creation request.
/// </summary>
public class CreateCommunityContentItemResult
{
	/// <summary>
	/// Externally determined identifier, or a GUID if not provided. This is saved alongside the Phrase to allow
	/// retrieval by a secondary key.
	/// </summary>
	public string? ExternalId { get; set; }

	/// <summary>
	/// Identifier of the Phrase.
	/// </summary>
	public int CommunityContentItemId { get; set; }
	
	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link>? Links { get; set; }

	public bool NewCollectionCreated { get; set; }

	public int CommunityContentCollectionId { get; set; }
	
}