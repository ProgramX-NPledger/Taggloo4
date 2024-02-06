using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with Languages.
/// </summary>
public interface ILanguageRepository
{
	
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="language">Entity to mark as updated.</param>
	void Update(Language language);
	
	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="language">Entity to create.</param>
	void Create(Language language);
	
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
	Task<Language?> GetLanguageByIetfLanguageTag(string ietfLanguageTag);
	

}