using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with Words in Phrases.
/// </summary>
public interface IWordInPhraseRepository
{
	/// <summary>
	/// Retrieves all <see cref="WordInPhrase"/> items.
	/// </summary>
	/// <returns>A collection of <see cref="WordInPhrase"/> items.</returns>
	Task<IEnumerable<WordInPhrase>> GetWordsInPhrasesAsync();
	
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
	

}