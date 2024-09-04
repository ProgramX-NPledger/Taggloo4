using Taggloo4.Web.Model;

namespace Taggloo4.Web.Contract;

/// <summary>
/// Represents an abstraction for working with Words.
/// </summary>
public interface IWordRepository
{
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="word">Entity to mark as updated.</param>
	void Update(Word word);
	
	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="word">Entity to create.</param>
	void Create(Word word);

	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();

	/// <summary>
	/// Retrieves a all matching <seealso cref="Word"/>s within a <seealso cref="Dictionary"/>.
	/// </summary>
	/// <param name="word">Word to match within the <seealso cref="Dictionary"/>.</param>
	/// <param name="dictionaryId">The ID of the <seealso cref="Dictionary"/> to search.</param>
	/// <param name="externalId">An externally determined identifier.</param>
	/// <returns>A collection of matching <seealso cref="Word"/>s within the <seealso cref="Dictionary"/>.</returns>
	Task<IEnumerable<Word>> GetWordsAsync(string? word, int? dictionaryId, string? externalId);
	
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
}