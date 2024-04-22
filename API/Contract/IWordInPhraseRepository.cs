using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with Words in Phrases.
/// </summary>
public interface IWordInPhraseRepository
{
	Task<IEnumerable<WordInPhrase>> GetWordsInPhrasesAsync();
	void Remove(WordInPhrase wordInPhrase);
	Task<bool> SaveAllAsync();
	

}