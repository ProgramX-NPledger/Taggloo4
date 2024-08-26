using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with Translators.
/// </summary>
public interface ITranslatorRepository
{
	/// <summary>
	/// Retrieves all matching <seealso cref="Translator"/>s.
	/// </summary>
	/// <param name="key">If specified, filters by <c>Key</c>.</param>
	/// <param name="isEnabled">If specified, filters by whether the Translator is enabled for use.</param>
	/// <returns>A collection of matching <seealso cref="Translator"/>s.</returns>
	Task<IEnumerable<Translator>> GetAllTranslatorsAsync(string? key, bool? isEnabled);

}