using Taggloo4.Model;

namespace Taggloo4.Contract;

/// <summary>
/// Represents an abstraction for working with Languages.
/// </summary>
public interface ILanguageRepository : IRepositoryBase<Language>
{

	
	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();
	
	/// <summary>
	/// Retrieves all Languages.
	/// </summary>
	/// <returns>A collection of Languages.</returns>
	Task<IEnumerable<Language>> GetAllLanguagesAsync();

	/// <summary>
	/// Retrieves a Language by its IETF Language Tag.
	/// </summary>
	/// <param name="ietfLanguageTag">The IETF Language Tag of the Dictionary.</param>
	/// <returns>The requested Language, or <c>null</c> if no Language could be found./</returns>
	Task<Language?> GetLanguageByIetfLanguageTagAsync(string ietfLanguageTag);

	/// <summary>
	/// Retrieves a all matching <seealso cref="Language"/>s.
	/// </summary>
	/// <param name="ietfLanguageTag">If specified, the IETF Language Tag of the <seealso cref="Language"/>.</param>
	/// <returns>A collection of matching <seealso cref="Language"/>s.</returns>
	Task<IEnumerable<Language>> GetLanguagesAsync(string? ietfLanguageTag);

}