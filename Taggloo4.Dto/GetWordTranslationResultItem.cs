
namespace Taggloo4.Dto;

/// <summary>
/// Represents a result within <seealso cref="GetWordTranslationsResult"/>.
/// </summary>
public class GetWordTranslationResultItem
{
	/// <summary>
	/// Identifier of Word.
	/// </summary>
	public int Id { get; set; }
	
	
	/// <summary>
	/// Links associated with the Word.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
	/// <summary>
	/// UserName of creator of the Word.
	/// </summary>
	public required string? CreatedByUserName { get; set; }

	/// <summary>
	/// Timestamp of creation of the Word.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// Host from which the Word was created.
	/// </summary>
	public required string CreatedOn { get; set; }
	
	/// <summary>
	/// Identifier of owning <seealso cref="Dictionary"/>.
	/// </summary>
	public int DictionaryId { get; set; }
	
	/// <summary>
	/// The IETF Language Tag for the Language this Word is in
	/// </summary>
	public string? FromIetfLanguageTag { get; set; }
	
	/// <summary>
	/// The Word being translated.
	/// </summary>
	public string? FromWord { get; set; }
	
	/// <summary>
	/// Identifier of Word being translated.
	/// </summary>
	public int FromWordId { get; set; }
	
	/// <summary>
	/// Identifier of the translation of the Word being translated.
	/// </summary>
	public int ToWordId { get; set; }
	
	
}