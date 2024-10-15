using Microsoft.EntityFrameworkCore;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Translation.ContentTypes;

public class WordsContentTypeManager : IContentTypeManager
{
    private DataContext _dataContext;
    private Dictionary _dictionary;
    
    public void Initialise(DataContext dataContext, Dictionary dictionary)
    {
        _dataContext = dataContext;
        _dictionary = dictionary;
    }

    public async Task DeleteDictionaryAndContentsAsync()
    {
        
        _dataContext.Dictionaries.RemoveRange(_dictionary);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<int> GetNumberOfItemsAsync()
    {
        return await _dataContext.Words
            .CountAsync(q=>(q.Dictionaries ?? Enumerable.Empty<Dictionary>()).Select(qq=>qq.Id).Contains(_dictionary.Id));
    }
}