using Taggloo4.Web.Model;

namespace Taggloo4.Web.Contract;

/// <summary>
/// Represents an abstraction for working with Translators.
/// </summary>
public interface ITranslatorConfigurationRepository
{

	/// <summary>
	/// Retrieves configurational data for a Translator.
	/// </summary>
	/// <param name="translatorKey">The key for the Translator.</param>
	/// <returns>Configurational data for a Translator, or <c>null</c> if no configuration exists.</returns>
	Task<TranslatorConfiguration?> GetTranslatorConfiguration(string translatorKey);
	
}