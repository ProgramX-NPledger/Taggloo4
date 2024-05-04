namespace API.Model;

/// <summary>
/// A <see cref="Word"/> within a <see cref="Phrase"/>.
/// </summary>
public class WordInPhrase
{
    /// <summary>
    /// Identifier of item.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Identifier of Phrase.
    /// </summary>
    public int InPhraseId { get; set; }
    
    /// <summary>
    /// Identifier of Word.
    /// </summary>
    public int WordId { get; set; }
    
    /// <summary>
    /// The ordinal of the Word in the Phrase. This can be used to infer relationships between Words.
    /// </summary>
    public int Ordinal { get; set; }
    
    /// <summary>
    /// The <see cref="Phrase"/> in which the <see cref="Word"/> appears.
    /// </summary>
    public required Phrase InPhrase { get; set; }
    
    /// <summary>
    /// The <see cref="Word"/>.
    /// </summary>
    public required Word Word { get; set; }
    
    /// <summary>
    /// The User name of the creating user.
    /// </summary>
    public required string CreatedByUserName { get; set; }
    
    /// <summary>
    /// The machine on which the item was created.
    /// </summary>
    public required string CreatedOn { get; set; }
    
    /// <summary>
    /// Timestamp of creation.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    
}