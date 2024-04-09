using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Model;

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
	public required string TheWord { get; set; }
	
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
	/// The owning <seealso cref="Dictionary"/>.
	/// </summary>
	public Dictionary? Dictionary { get; set; }
	
	/// <summary>
	/// Identifier of owning <seealso cref="Dictionary"/>.
	/// </summary>
	public int DictionaryId { get; set; }

	public ICollection<WordTranslation> Translations { get; set; }

	/// <summary>
	/// Phrases in which this Word appears.
	/// </summary>
	public ICollection<Phrase>? Phrases { get; set; }

	/// <summary>
	/// An external identifier that can be applied to the entity for ready identification.
	/// </summary>
	[MaxLength(32)]
	public string? ExternalId { get; set; }
	
}