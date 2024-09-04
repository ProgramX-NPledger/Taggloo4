namespace Taggloo4.Web.Translation.Translators.Results;

/// <summary>
/// Represents a result from the <seealso cref="PhraseTranslator"/>.
/// </summary>
public class PhraseTranslationResultItem : TranslationResultItem
{
    /// <summary>
    /// Identifier of Phrase being translated.
    /// </summary>
    public int FromPhraseId { get; set; }
    
    /// <summary>
    /// Identifier of Phrase that is the translation.
    /// </summary>
    public int ToPhraseId { get; set; }

    /// <summary>
    /// Phrase being translated from.
    /// </summary>
    public required string FromPhrase { get; set; }

    /// <summary>
    /// Percentage match of the translated Phrase.
    /// </summary>
    public int? PercentageMatch { get; set; }
}