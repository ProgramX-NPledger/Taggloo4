namespace API.Translation;

/// <summary>
/// Provides Translation services for specific content.
/// </summary>
public interface ITranslator
{
    TranslationResults Translate(TranslationRequest translationRequest);
}