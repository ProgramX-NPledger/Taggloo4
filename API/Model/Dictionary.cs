using System.ComponentModel.DataAnnotations;

namespace API.Model;

/// <summary>
/// A Dictionary has a Language and content such as Words, Phrases, Translations, etc.
/// </summary>
public class Dictionary
{
	/// <summary>
	/// Identifier of Dictionary.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Name of Dictionary.
	/// </summary>
	[Required]
	[MaxLength(128)]
	public required string Name { get; set; }

	/// <summary>
	/// Description of Dictionary
	/// </summary>
	[MaxLength(1024)]
	public required string Description { get; set; }

	/// <summary>
	/// URL of source of Dictionary.
	/// </summary>
	[MaxLength(1024)]
	public required string SourceUrl { get; set; }

	/// <summary>
	/// IETF Language-tag of the Dictionary. This must be a valid Language.
	/// </summary>
	public required string IetfLanguageTag { get; set; }

	/// <summary>
	/// UserName of creator.
	/// </summary>
	[MaxLength(128)]
	public required string? CreatedByUserName { get; set; }

	/// <summary>
	/// Timestamp of creation.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// Host from which the Dictionary was created.
	/// </summary>
	[MaxLength(256)]
	public required string CreatedOn { get; set; }

	/// <summary>
	/// <seealso cref="Language"/> of Dictionary.
	/// </summary>
	public Language? Language { get; set; }

	/// <summary>
	/// <seealso cref="Word"/>s in Dictionary.
	/// </summary>
	public ICollection<Word>? Words { get; set; }

	/// <summary>
	/// <seealso cref="WordTranslation"/>s in Dictionary.
	/// </summary>
	public ICollection<WordTranslation>? WordTranslations { get; set; }

	/// <summary>
	/// <seealso cref="Phrase"/>s in Dictionary
	/// </summary>
	public ICollection<Phrase>? Phrases { get; set; }

	/// <summary>
	/// <seealso cref="PhraseTranslation"/>s in Dictionary.
	/// </summary>
	public ICollection<PhraseTranslation>? PhraseTranslations { get; set; }

	/// <summary>
	/// The URL name of the Controller to use to retrieve content.
	/// </summary>
	[MaxLength(32)]
	public required string Controller { get; set; }

	/// <summary>
	/// Disambiguated identifier for type of content to allow automatic processing.
	/// </summary>
	[MaxLength(32)]
	public required string ContentTypeKey { get; set; }

	/// <summary>
	/// Human-suitable name of Content Type.
	/// </summary>
	[MaxLength(128)]
	public required string ContentTypeFriendlyName { get; set; }
}
	
	
	
	