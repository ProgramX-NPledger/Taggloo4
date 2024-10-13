using Taggloo4.Contract;
using Taggloo4.Model;
using Taggloo4.Translation;

namespace Taggloo4.Web.Hangfire.Jobs;

/// <summary>
/// Job to delete a Dictionary and all included content.
/// </summary>
public class DeleteDictionaryJob
{
    private readonly IDictionaryRepository _dictionaryRepository;

    /// <summary>
    /// Constructor with injected parameters.
    /// </summary>
    /// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
    public DeleteDictionaryJob(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }
    
    /// <summary>
    /// Deletes a Dictionary.
    /// </summary>
    /// <param name="dictionaryId">ID of the Dictionary to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown if the Dictionary cannot be deleted.</exception>
    public void DeleteDictionary(int dictionaryId)
    {
        // check if Dictionary exists
        Dictionary? dictionary = _dictionaryRepository.GetByIdAsync(dictionaryId).Result;
        if (dictionary == null) throw new InvalidOperationException($"Attempt to delete non-existent Dictionary");

        try
        {
            // get the Dictionary Manager for the Dictionary
            IDictionaryManager dictionaryManager = DictionaryManagerFactory.CreateDictionaryManager(dictionary);
            
            // have the Dictionary Manager delete the Dictionary
            dictionaryManager.DeleteDictionaryAndContentsAsync().Wait();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Failure to delete Dictionary", e);
        }
        
    }
}