using Taggloo4.Model;

namespace Taggloo4.Contract;

/// <summary>
/// Represents an abstraction for working with Phrases.
/// </summary>
public interface IPhraseRepository : IRepositoryBase<Phrase>
{


	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();

	/// <summary>
	/// Retrieves a all matching <seealso cref="Phrase"/>s within a <seealso cref="Dictionary"/>.
	/// </summary>
	/// <param name="phrase">Word to match within the <seealso cref="Dictionary"/>.</param>
	/// <param name="dictionaryId">The ID of the <seealso cref="Dictionary"/> to search.</param>
	/// <param name="containingText">Filters response for presence of text (collation as per database).</param>
	/// <param name="externalId">If specified, filters using an externally determined identifier.</param>
	/// <param name="languageCode">If specified, filters within Dictionaries for the IETF Language Tag.</param>
	/// <returns>A collection of matching <seealso cref="Phrase"/>s within the <seealso cref="Dictionary"/>.</returns>
	Task<IEnumerable<Phrase>> GetPhrasesAsync(string? phrase, int? dictionaryId, string? containingText, string? externalId, string? languageCode);
	
	/// <summary>
	/// Retrieves a <seealso cref="Phrase"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Phrase"/>.</param>
	/// <returns>The requested <seealso cref="Phrase"/>, or <c>null</c> if no Phrase could be found./</returns>
	Task<Phrase?> GetByIdAsync(int id);
	

	


}