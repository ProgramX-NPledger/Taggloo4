using System.ComponentModel.DataAnnotations;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Translation;

namespace Taggloo4.Web.Model;

/// <summary>
/// Configurational data for a Translator.
/// </summary>
public class TranslatorConfiguration : ITranslatorConfiguration
{
    /// <summary>
    /// A unique identifier of the Translator. This should be equivalent to a call to `nameof()` for the
    /// implementing type of the <seealso cref="ITranslator"/>.
    /// </summary>
    [Key]
    public required string Key { get; set; }
    
    /// <summary>
    /// Whether the Translator is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <inheritdoc cref="ITranslatorConfiguration.Priority"/>
    public int Priority { get; set; }

    /// <inheritdoc cref="ITranslatorConfiguration.NumberOfItemsInSummary"/>
    public int NumberOfItemsInSummary { get; set;  }
}