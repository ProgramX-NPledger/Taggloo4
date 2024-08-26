namespace API.Translation;

/// <summary>
/// Returned by implementations of <seealso cref="ITranslator"/>.
/// </summary>
public class TranslationResults
{
    /// <summary>
    /// A list of results provided by the implementation of <seealso cref="ITranslator"/>. If this is <c>null</c>,
    /// the query was inappropriate for the translation and no output should be rendered.
    /// </summary>
    public IEnumerable<TranslationResultItem>? ResultItems { get; set; }

    public int MaximumItems { get; set; } = 5;

    public int? NumberOfAvailableItemsBeforePaging { get; set; }

    

}