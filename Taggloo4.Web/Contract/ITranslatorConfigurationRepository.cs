using Taggloo4.Web.Model;

namespace Taggloo4.Web.Contract;

/// <summary>
/// Represents an abstraction for working with Translators.
/// </summary>
public interface ITranslatorConfigurationRepository
{
	/// <summary>
	/// Retrieves all matching <seealso cref="Translator"/>s.
	/// </summary>
	/// <param name="key">If specified, filters by <c>Key</c>.</param>
	/// <param name="isEnabled">If specified, filters by whether the Translator is enabled for use.</param>
	/// <returns>A collection of matching <seealso cref="Translator"/>s.</returns>
	Task<IEnumerable<TranslatorConfiguration>> GetAllTranslatorsAsync(string? key, bool? isEnabled);

	
}