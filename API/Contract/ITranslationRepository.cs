using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with Translations.
/// </summary>
public interface ITranslationRepository
{
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="wordTranslation">Entity to mark as updated.</param>
	void Update(WordTranslation wordTranslation);
	
	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="wordTranslation">Entity to create.</param>
	void Create(WordTranslation wordTranslation); // overload this for different translation types
	
	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();


	/// <summary>
	/// Retrieves a <seealso cref="WordTranslation"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="WordTranslation"/>.</param>
	/// <returns>The requested <seealso cref="WordTranslation"/>, or <c>null</c> if no Word Translation could be found./</returns>
	Task<WordTranslation?> GetById(int id);
	

}