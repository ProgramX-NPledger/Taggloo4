namespace API.Translation;

/// <summary>
/// Returned by implementations of <seealso cref="ITranslator"/>.
/// </summary>
public class TranslationResults
{
    /// <summary>
    /// A list of results provided by the implementation of <seealso cref="ITranslator"/>.
    /// </summary>
    public IEnumerable<dynamic> ResultItems { get; set; }
    
}