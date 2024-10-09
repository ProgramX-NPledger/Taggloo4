namespace Taggloo4.Contract;

public interface IDictionaryManager
{
    Task DeleteDictionaryAndContentsAsync();
    Task<int> GetNumberOfItemsAsync();
}