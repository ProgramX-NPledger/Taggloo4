using Taggloo4.Contract.Criteria;
using Taggloo4.Model;

namespace Taggloo4.Contract;

/// <summary>
/// Represents an abstraction for working with Dictionaries.
/// </summary>
public interface IDictionaryRepository : IRepositoryBase<Dictionary>
{

	
	
	
	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();

	/// <summary>
	/// Retrieves a <seealso cref="Dictionary"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Dictionary"/>.</param>
	/// <returns>The requested <seealso cref="Dictionary"/>, or <c>null</c> if no Dictionary could be found./</returns>
	Task<Dictionary?> GetByIdAsync(int id);

	/// <summary>
	/// Retrieves all matching <seealso cref="Dictionary"/> items.
	/// </summary>
	/// <param name="id">If specified, matches by the ID of the Dictionary.</param>
	/// <param name="ietfLanguageTag">If specified, limits by IETF Language Tag</param>
	/// <returns>A collection of matching <seealso cref="Dictionary"/> items.</returns>
	Task<IEnumerable<Dictionary>> GetDictionariesAsync(int? id, string? ietfLanguageTag);

	/// <summary>
	/// Get Dictionaries matching criteria.
	/// </summary>
	/// <param name="criteria">Criteria of Dictionaries to return.</param>
	/// <returns>List of <seealso cref="Dictionary"/> items.</returns>
	Task<PagedResults<DictionaryWithContentTypeAndLanguage>> GetDictionariesByCriteriaAsync(GetDictionariesCriteria criteria);
	
	/// <summary>
	/// Deletes the specified Dictionary, and all related content.
	/// </summary>
	/// <param name="dictionaryId">ID of Dictionary to delete.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Thrown if deletion fails.</exception>
	Task Delete(int dictionaryId);


	/// <summary>
	/// Return all Content Types.
	/// </summary>
	/// <returns>A collection of <seealso cref="ContentType"/> items which may relate to a Dictionary.</returns>
	Task<IEnumerable<ContentType>> GetAllContentTypes();

}