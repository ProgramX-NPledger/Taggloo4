namespace Taggloo4.Dto;

/// <summary>
/// Represents a result within <seealso cref="GetPhrasesResult"/>.
/// </summary>
public class GetPhrasesResultItem
{
	/// <summary>
	/// Identifier of Phrase.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// The Phrase.
	/// </summary>
	public required string Phrase { get; set; }
	
	/// <summary>
	/// Links associated with the Word.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
}