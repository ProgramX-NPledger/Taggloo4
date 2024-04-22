using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Represents a repository for working with Words in Phrases.
/// </summary>
public class WordInPhraseRepository : IWordInPhraseRepository
{
    private readonly DataContext _dataContext;

    /// <summary>
    /// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
    /// </summary>
    /// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
    public WordInPhraseRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    /// <summary>
    /// Retrieves all <see cref="WordInPhrase"/> items.
    /// </summary>
    /// <returns>A collection of <see cref="WordInPhrase"/> items.</returns>
    public async Task<IEnumerable<WordInPhrase>> GetWordsInPhrasesAsync()
    {
        return await _dataContext.WordsInPhrases
            .Include("Word")
            .Include("InPhrase")
            .ToArrayAsync();
    }

    /// <summary>
    /// Removes the <see cref="WordInPhrase"/>.
    /// </summary>
    /// <param name="wordInPhrase"><see cref="WordInPhrase"/> to remove.</param>
    public void Remove(WordInPhrase wordInPhrase)
    {
        _dataContext.WordsInPhrases.Remove(wordInPhrase);
    }

    /// <summary>
    /// Saves changes to the data store.
    /// </summary>
    /// <returns><c>True</c> if successful.</returns>
    public async Task<bool> SaveAllAsync()
    {
        return await _dataContext.SaveChangesAsync() > 0;
    }
}