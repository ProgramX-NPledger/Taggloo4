
namespace Taggloo4.Dto;

/// <summary>
/// Represents a result within <seealso cref="GetPhraseTranslationsResult"/>.
/// </summary>
public class GetPhraseTranslationResultItem
{
	/// <summary>
	/// Identifier of Phrase Translation.
	/// </summary>
	public int Id { get; set; }
	
	
	/// <summary>
	/// Links associated with the Phrase Translation.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
	/// <summary>
	/// UserName of creator of the Phrase Translation.
	/// </summary>
	public required string? CreatedByUserName { get; set; }

	/// <summary>
	/// Timestamp of creation of the Phrase Translation.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// Host from which the Phrase Translation was created.
	/// </summary>
	public required string CreatedOn { get; set; }
	
	/// <summary>
	/// Identifier of owning <seealso cref="Dictionary"/>.
	/// </summary>
	public int DictionaryId { get; set; }
	
	/// <summary>
	/// The IETF Language Tag for the Language this Phrase Translation is in
	/// </summary>
	public string? FromIetfLanguageTag { get; set; }
	
	/// <summary>
	/// The Phrase being translated.
	/// </summary>
	public string? FromPhrase { get; set; }
	
	/// <summary>
	/// Identifier of Phrase being translated.
	/// </summary>
	public int FromPhraseId { get; set; }
	
	/// <summary>
	/// Identifier of the translation of the Phrase being translated.
	/// </summary>
	public int ToPhraseId { get; set; }
	
	
}