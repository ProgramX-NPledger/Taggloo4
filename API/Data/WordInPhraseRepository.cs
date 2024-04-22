using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class WordInPhraseRepository : IWordInPhraseRepository
{
    private readonly DataContext _dataContext;

    public WordInPhraseRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<IEnumerable<WordInPhrase>> GetWordsInPhrasesAsync()
    {
        return await _dataContext.WordsInPhrases
            .Include("Word")
            .Include("InPhrase")
            .ToArrayAsync();
    }

    public void Remove(WordInPhrase wordInPhrase)
    {
        _dataContext.WordsInPhrases.Remove(wordInPhrase);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _dataContext.SaveChangesAsync() > 0;
    }
}