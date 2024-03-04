using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Model;

/// <summary>
/// A Phrase within a <seealso cref="Dictionary"/>.
/// </summary>
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

	/// <summary>
	/// The owning <seealso cref="Dictionary"/>.
	/// </summary>
	public Dictionary? Dictionary { get; set; }
	
	/// <summary>
	/// Identifier of owning <seealso cref="Dictionary"/>.
	/// </summary>
	public int DictionaryId { get; set; }
	
	/// <summary>
	/// Words used in this Phrase.
	/// </summary>
	public ICollection<Word>? Words { get; set; }
	
	// this fails when building migrations
//	public ICollection<WordTranslation> Translations { get; set; }

	
	
}