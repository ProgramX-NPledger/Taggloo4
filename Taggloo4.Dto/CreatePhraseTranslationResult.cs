using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a Phrase Trannslation creation request.
/// </summary>
public class CreatePhraseTranslationResult
{
	/// <summary>
	/// Internal identification of Phrase Translation
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link>? Links { get; set; }
	
}