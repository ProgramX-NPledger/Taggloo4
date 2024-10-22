using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Model;

/// <summary>
/// A Word within a <seealso cref="Dictionary"/>.
/// </summary>
public class Word
{
	/// <summary>
	/// The identifier of the Word.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// The Word.
	/// </summary>
	[Required] 
	[MaxLength(450)]
	public required string TheWord { get; set; }
	
	/// <summary>
	/// UserName of creator of the Word.
	/// </summary>
	public required string CreatedByUserName { get; set; }

	/// <summary>
	/// Timestamp of creation of the Word.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// Host from which the Word was created.
	/// </summary>
	public required string CreatedOn { get; set; }

	/// <summary>
	/// The owning <seealso cref="Dictionary"/>.
	/// </summary>
	public required ICollection<Dictionary> Dictionaries { get; set; } = [];


	/// <summary>
	/// Translations of the Word.
	/// </summary>
	public ICollection<WordTranslation> ToTranslations { get; set; } = new List<WordTranslation>();

	/// <summary>
	/// Translations of the Word.
	/// </summary>
	public ICollection<WordTranslation> FromTranslations { get; set; } = new List<WordTranslation>();

	/// <summary>
	/// Phrases in which this Word appears.
	/// </summary>
	public ICollection<Phrase>? Phrases { get; set; }

	/// <summary>
	/// An external identifier that can be applied to the entity for ready identification.
	/// </summary>
	[MaxLength(32)]
	public string? ExternalId { get; set; }

	/// <summary>
	/// Appearances of this Word in <see cref="Phrase"/>s.
	/// </summary>
	public ICollection<WordInPhrase> AppearsInPhrases { get; set; } = new List<WordInPhrase>();

}