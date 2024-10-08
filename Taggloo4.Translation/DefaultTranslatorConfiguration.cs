﻿using Taggloo4.Contract.Translation;

namespace Taggloo4.Translation;

/// <summary>
/// Default configuration applied to a Translator when a defined configuration cannot be resolved.
/// </summary>
public class DefaultTranslatorConfiguration : ITranslatorConfiguration
{
    /// <summary>
    /// Whether the Translator is enabled for use. Always <c>True</c>.
    /// </summary>
    public bool IsEnabled { get; } = true;

    /// <summary>
    /// Priority of the Translator. Always a fixed number.
    /// </summary>
    public int Priority { get; } = int.MaxValue;

    /// <inheritdoc cref="ITranslatorConfiguration.NumberOfItemsInSummary"/>
    public int NumberOfItemsInSummary { get; } = 5;
}