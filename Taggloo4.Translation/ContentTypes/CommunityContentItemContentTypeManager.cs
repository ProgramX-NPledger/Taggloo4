using Microsoft.EntityFrameworkCore;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Translation.ContentTypes;

public class CommunityContentItemContentTypeManager : IContentTypeManager
{
    private DataContext _dataContext;
    private Dictionary _dictionary;

    private CommunityContentItemContentTypeManager(DataContext dataContext, Dictionary dictionary)
    {
        _dataContext = dataContext;
        _dictionary = dictionary;
    }

    public IContentTypeManager Initialise(DataContext dataContext, Dictionary dictionary)
    {
        return new CommunityContentItemContentTypeManager(dataContext, dictionary);
    }

    public async Task DeleteDictionaryAndContentsAsync()
    {
        _dataContext.Dictionaries.RemoveRange(_dictionary);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<int> GetNumberOfItemsAsync()
    {
        return await _dataContext.CommunityContentItems
            .CountAsync(q=>q.DictionaryId == _dictionary.Id);
    }
}