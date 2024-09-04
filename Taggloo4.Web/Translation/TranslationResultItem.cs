namespace Taggloo4.Web.Translation;

/// <summary>
/// A base class for result items from a <seealso cref="ITranslator"/>.
/// </summary>
/// <remarks>
/// This class should be derived for the purposes of the <seealso cref="ITranslator"/>, providing
/// more relevant results.
/// </remarks>
public class TranslationResultItem
{
    /// <summary>
    /// The translation.
    /// </summary>
    public string? Translation { get; set; }
}
