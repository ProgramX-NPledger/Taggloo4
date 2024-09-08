using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Words in Phrases.
/// </summary>
public class WordInPhraseRepository : RepositoryBase<WordInPhrase>, IWordInPhraseRepository
{
    /// <summary>
    /// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
    /// </summary>
    /// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
    public WordInPhraseRepository(DataContext dataContext) : base(dataContext)
    {
    }
    
    /// <summary>
    /// Retrieves all matching <see cref="WordInPhrase"/> items.
    /// </summary>
    /// <param name="wordId">If specified, filters by the provided Word ID.</param>
    /// <param name="phraseId">If specified, filters by the provided Phrase ID.</param>
    /// <returns>A collection of <see cref="WordInPhrase"/> items sorted by Phrase ID and Ordinal.</returns>
    public async Task<IEnumerable<WordInPhrase>> GetWordsInPhrasesAsync(
        int? wordId,
        int? phraseId)
    {
        IQueryable<WordInPhrase> query = DataContext.WordsInPhrases
            .Include("Word")
            .Include("InPhrase")
            .AsQueryable();

        if (wordId.HasValue)
        {
            query = query.Where(q => q.WordId == wordId.Value);
        }

        if (phraseId.HasValue)
        {
            query = query.Where(q => q.InPhraseId == phraseId.Value);
        }

        return await query.OrderBy(q=>q.InPhraseId).ThenBy(q=>q.Ordinal).ToArrayAsync();
    }

    /// <summary>
    /// Removes the <see cref="WordInPhrase"/>.
    /// </summary>
    /// <param name="wordInPhrase"><see cref="WordInPhrase"/> to remove.</param>
    [Obsolete("Use Delete from RepositoryBase<T> instead")]
    public void Remove(WordInPhrase wordInPhrase)
    {
        DataContext.WordsInPhrases.Remove(wordInPhrase);
    }

    /// <summary>
    /// Saves changes to the data store.
    /// </summary>
    /// <returns><c>True</c> if successful.</returns>
    public async Task<bool> SaveAllAsync()
    {
        return await DataContext.SaveChangesAsync() > 0;
    }
    
    /// <summary>
    /// Retrieves the requested <see cref="WordInPhrase"/> items.
    /// </summary>
    /// <param name="id">Identifier of item to return.</param>
    /// <returns>A collection of <see cref="WordInPhrase"/> items.</returns>
    public async Task<WordInPhrase?> GetByIdAsync(int id)
    {
        return await DataContext.WordsInPhrases
            .Include("Word")
            .Include("InPhrase")
            .SingleOrDefaultAsync(q => q.Id == id);
    }

    
}