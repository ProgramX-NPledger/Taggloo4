using Taggloo4.Contract.Criteria;
using Taggloo4.Model;

namespace Taggloo4.Contract;

/// <summary>
/// Represents an abstraction for working with Words.
/// </summary>
public interface IWordRepository : IRepositoryBase<Word>
{


	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();

	/// <summary>
	/// Retrieves all matching <seealso cref="Word"/>s within a <seealso cref="Dictionary"/>.
	/// </summary>
	/// <param name="word">Word to match within the <seealso cref="Dictionary"/>.</param>
	/// <param name="dictionaryId">The ID of the <seealso cref="Dictionary"/> to search.</param>
	/// <param name="externalId">An externally determined identifier.</param>
	/// <param name="ietfLanguageTag">If specified, filters by the IETF Language Tag.</param>
	/// <returns>A collection of matching <seealso cref="Word"/>s within the <seealso cref="Dictionary"/>.</returns>
	Task<IEnumerable<Word>> GetWordsAsync(string? word, int? dictionaryId, string? externalId, string? ietfLanguageTag);
	
	/// <summary>
	/// Retrieves a <seealso cref="Word"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Word"/>.</param>
	/// <returns>The requested <seealso cref="Word"/>, or <c>null</c> if no Word could be found./</returns>
	Task<Word?> GetByIdAsync(int id);

	/// <summary>
	/// Retrieves matching <see cref="WordInPhrase"/> items.
	/// </summary>
	/// <param name="wordId">The ID of the <see cref="Word"/>.</param>
	/// <param name="phraseId">The ID of the <see cref="Phrase"/>.</param>
	/// <param name="ordinal">The ordinal of the Word.</param>
	/// <returns>A collection of matching <see cref="WordInPhrase"/>s.</returns>
	Task<IEnumerable<WordInPhrase>> GetPhrasesForWordAsync(int wordId, int phraseId, int ordinal);

	/// <summary>
	/// Adds a <see cref="WordInPhrase"/>, representing the <see cref="Word"/> appearing in a <see cref="Phrase"/>.
	/// </summary>
	/// <param name="wordInPhrase">The created <see cref="WordInPhrase"/> item.</param>
	void AddPhraseForWord(WordInPhrase wordInPhrase);

	/// <summary>
	/// Get summary information about Words in the Taggloo database.
	/// </summary>
	/// <returns>A <seealso cref="WordsSummary"/> containing summary data.</returns>
	Task<WordsSummary> GetWordsSummaryAsync();

	/// <summary>
	/// Get Words matching criteria.
	/// </summary>
	/// <param name="criteria">Criteria of Words to return.</param>
	/// <returns>List of <seealso cref="WordInDictionary"/> items.</returns>
	Task<PagedResults<WordInDictionary>> GetWordsByCriteriaAsync(GetWordsCriteria criteria);

}