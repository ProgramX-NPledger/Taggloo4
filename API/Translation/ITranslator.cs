namespace API.Translation;

/// <summary>
/// Provides Translation services for specific content.
/// </summary>
public interface ITranslator
{
    /// <summary>
    /// Perform the translation.
    /// </summary>
    /// <param name="translationRequest">The requested translation.</param>
    /// <returns>The results of the translation.</returns>
    TranslationResults Translate(TranslationRequest translationRequest);
    
}