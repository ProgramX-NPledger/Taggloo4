using Taggloo4.Data.EntityFrameworkCore;

namespace Taggloo4.Translation;

public interface IContentTypeManager
{
    Task InitialiseAsync(DataContext dataContext);
    Task DeleteDictionaryAndContentsAsync();
    Task<int> GetNumberOfItemsAsync();
}