using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with Words in Phrases.
/// </summary>
public interface IWordInPhraseRepository
{
	/// <summary>
	/// Retrieves all matching <see cref="WordInPhrase"/> items.
	/// </summary>
	/// <param name="wordId">If specified, filters by the provided Word ID.</param>
	/// <param name="phraseId">If specified, filters by the provided Phrase ID.</param>
	/// <returns>A collection of <see cref="WordInPhrase"/> items sorted by Phrase ID and Ordinal.</returns>

	Task<IEnumerable<WordInPhrase>> GetWordsInPhrasesAsync(int? wordId, int? phraseId);
	
	/// <summary>
	/// Removes the <see cref="WordInPhrase"/>.
	/// </summary>
	/// <param name="wordInPhrase"><see cref="WordInPhrase"/> to remove.</param>
	void Remove(WordInPhrase wordInPhrase);
	
	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();

	/// <summary>
	/// Retrieves the requested <see cref="WordInPhrase"/> items.
	/// </summary>
	/// <param name="id">Identifier of item to return.</param>
	/// <returns>A collection of <see cref="WordInPhrase"/> items.</returns>
	Task<WordInPhrase?> GetByIdAsync(int id);


}