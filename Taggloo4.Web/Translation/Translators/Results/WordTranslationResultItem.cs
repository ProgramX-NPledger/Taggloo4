namespace Taggloo4.Web.Translation.Translators.Results;

/// <summary>
/// Represents a result from the <seealso cref="WordTranslator"/>.
/// </summary>
public class WordTranslationResultItem : TranslationResultItem
{
    /// <summary>
    /// Identifier of Word being translated.
    /// </summary>
    public int FromWordId { get; set; }
    
    /// <summary>
    /// Identifier of Word that is the translation.
    /// </summary>
    public int ToWordId { get; set; }
}