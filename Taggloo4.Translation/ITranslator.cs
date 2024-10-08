﻿using Taggloo4.Contract.Translation;

namespace Taggloo4.Translation;

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
    
    /// <summary>
    /// Configuration for the Translator.
    /// </summary>
    ITranslatorConfiguration Configuration { get; }
    
}