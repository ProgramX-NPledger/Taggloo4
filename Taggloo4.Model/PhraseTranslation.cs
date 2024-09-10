namespace Taggloo4.Model;

/// <summary>
/// Translation of a <seealso cref="Phrase"/>.
/// </summary>
public class PhraseTranslation
{
	/// <summary>
	/// Identifier of the Translation.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// The identifier of the <seealso cref="Phrase"/> being translated.
	/// </summary>
	public int FromPhraseId { get; set; }
	// this breaks migrations
	//public Word FromWord { get; set; }
	
	/// <summary>
	/// The identifier of the <seealso cref="Phrase"/> translation.
	/// </summary>
	public int ToPhraseId { get; set; }
	
	/// <summary>
	/// The Phrase Translation
	/// </summary>
	public Phrase? ToPhrase { get; set; }
	
	/// <summary>
	/// The Host from which the Phrase Translation was created.
	/// </summary>
	public string? CreatedOn { get; set; }
	
	/// <summary>
	/// The UserName of the creator of the Translation.
	/// </summary>
	public string? CreatedByUserName { get; set; }
	
	/// <summary>
	/// The timestamp of creation of the Translation.
	/// </summary>
	public DateTime CreatedAt { get; set; }
	
	/// <summary>
	/// Identifier of the <seealso cref="Dictionary"/> owner of the Translation.
	/// </summary>
	public int DictionaryId { get; set; }
	
	
	/// <summary>
	/// Owning <seealso cref="Dictionary"/> of the Translation.
	/// </summary>
	public Dictionary? Dictionary { get; set; }
	
	/// <summary>
	/// Phrase being translated.
	/// </summary>
	public Phrase? FromPhrase { get; set; }
	
	
}