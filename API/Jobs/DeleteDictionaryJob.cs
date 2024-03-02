using API.Contract;

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
        int i = 5;
    }
}