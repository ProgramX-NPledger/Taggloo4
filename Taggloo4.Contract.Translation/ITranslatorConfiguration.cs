namespace Taggloo4.Contract.Translation;

/// <summary>
/// Configuration for a Translator.
/// </summary>
public interface ITranslatorConfiguration
{
    /// <summary>
    /// Whether the Translator is enabled.
    /// </summary>
    bool IsEnabled { get; }
    
    /// <summary>
    /// Priority of the Translator, lower priority indicates higher appearance in sorting.
    /// </summary>
    /// <remarks>
    /// Used when ordering results of Translator results.
    /// </remarks>
    int Priority { get; }

    /// <summary>
    /// Number of items to return in summary view.
    /// </summary>    
    int NumberOfItemsInSummary { get; }

}