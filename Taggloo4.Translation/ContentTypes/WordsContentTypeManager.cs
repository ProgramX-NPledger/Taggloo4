using Taggloo4.Data.EntityFrameworkCore;

namespace Taggloo4.Translation.ContentTypes;

public class WordsContentTypeManager : IContentTypeManager
{
    private DataContext _dataContext;
    
    public async Task InitialiseAsync(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task DeleteDictionaryAndContentsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetNumberOfItemsAsync()
    {
        throw new NotImplementedException();
    }
}