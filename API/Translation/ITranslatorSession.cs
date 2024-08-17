﻿namespace API.Translation;

/// <summary>
/// Contract for implementation of translation across a number of <seealso cref="ITranslator"/> instances.
/// </summary>
public interface ITranslatorSession
{
    /// <summary>
    /// Performs Translation.
    /// </summary>
    /// <param name="translationRequest">Translation Request.</param>
    void Translate(TranslationRequest translationRequest);
}