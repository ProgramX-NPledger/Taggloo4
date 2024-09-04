namespace Taggloo4.Web.Contract;

/// <summary>
/// Configuration for a Translator.
/// </summary>
public interface ITranslatorConfiguration
{
    /// <summary>
    /// Whether the Translator is enabled.
    /// </summary>
    public bool IsEnabled { get; }
}