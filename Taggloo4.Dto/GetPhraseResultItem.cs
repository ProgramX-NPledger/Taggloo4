namespace Taggloo4.Dto;

/// <summary>
/// Represents a result within <seealso cref="GetPhrasesResult"/>.
/// </summary>
public class GetPhraseResultItem
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
	/// Links associated with the Phrase.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
	/// <summary>
	/// UserName of creator of the Phrase.
	/// </summary>
	public required string? CreatedByUserName { get; set; }

	/// <summary>
	/// Timestamp of creation of the Phrase.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// Host from which the Phrase was created.
	/// </summary>
	public required string CreatedOn { get; set; }
	
	/// <summary>
	/// Identifier of owning <see cref="Dictionary"/>.
	/// </summary>
	public int DictionaryId { get; set; }
	
	/// <summary>
	/// The unique identifier given to this Phrase during importing.
	/// </summary>
	public Guid? ImportId { get; set; }
	
}