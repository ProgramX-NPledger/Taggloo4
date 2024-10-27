using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taggloo4.Model;

/// <summary>
/// A Phrase within a Dictionary.
/// </summary>
/// <seealso cref="Dictionary"/>
public class Phrase
{
	/// <summary>
	/// The identifier of the Phrase.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// The Phrase.
	/// </summary>
	[Required] 
	public required string ThePhrase { get; set; }
	
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

	// TODO: for removal
	/// <summary>
	/// The owning Dictionary.
	/// </summary>
	/// <seealso cref="Dictionary"/>.
	[NotMapped]
	public Dictionary? Dictionary { get; set; }
	
	
	/// <summary>
	/// The owning <seealso cref="Dictionary"/>.
	/// </summary>
	public required ICollection<Dictionary> Dictionaries { get; set; } = [];
	
	/// <summary>
	/// Identifier of owning Dictionary.
	/// </summary>
	/// <seealso cref="Dictionary"/>
	public int DictionaryId { get; set; }
	
	/// <summary>
	/// Words used in this Phrase.
	/// </summary>
	public ICollection<Word>? Words { get; set; }

	/// <summary>
	/// Translations of the Phrase
	/// </summary>
	public ICollection<PhraseTranslation> FromTranslations { get; set; } = new List<PhraseTranslation>();

	/// <summary>
	/// Translations of the Phrase
	/// </summary>
	public ICollection<PhraseTranslation> Translations { get; set; } = new List<PhraseTranslation>();

	/// <summary>
	/// An external identifier that can be applied to the entity for ready identification.
	/// </summary>
	[MaxLength(32)]
	public string? ExternalId { get; set; }

	/// <summary>
	/// Appearances of <see cref="Word"/>s in this Phrase.
	/// </summary>
	public ICollection<WordInPhrase> HasWordsInPhrase { get; set; } = new List<WordInPhrase>();
}
	
