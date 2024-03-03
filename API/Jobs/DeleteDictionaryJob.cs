using API.Contract;
using API.Model;

namespace API.Jobs;

public class DeleteDictionaryJob
{
    private readonly IDictionaryRepository _dictionaryRepository;

    public DeleteDictionaryJob(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }
    
    public void DeleteDictionary(int dictionaryId)
    {
        // check if Dictionary exists
        Dictionary? dictionary = _dictionaryRepository.GetByIdAsync(dictionaryId).Result;
        if (dictionary == null) throw new InvalidOperationException($"Attempt to delete non-existent Dictionary");

        try
        {
            _dictionaryRepository.Delete(dictionaryId);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Failure to delete Dictionary", e);
        }
        
    }
}