using System.ComponentModel.DataAnnotations;
using API.Translation;

namespace API.Model;

/// <summary>
/// Configurational data for a Translator.
/// </summary>
public class Translator
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
    
}