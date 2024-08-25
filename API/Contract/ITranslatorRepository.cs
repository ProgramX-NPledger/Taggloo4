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
	/// <param name="dotNetFactoryType">If specified, filters by .NET Factory Type name.</param>
	/// <param name="dotNetFactoryAssembly">If specified, filters by .NET Factory Assembly Name.</param>
	/// <param name="isEnabled">If specified, filters by whether the Translator is enabled for use.</param>
	/// <returns>A collection of matching <seealso cref="Translator"/>s.</returns>
	Task<IEnumerable<Translator>> GetAllTranslatorsAsync(string? key, string? dotNetFactoryType, string? dotNetFactoryAssembly, bool? isEnabled);

}