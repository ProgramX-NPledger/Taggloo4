using Taggloo4.Model;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Jobs;

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
            _dictionaryRepository.Delete(dictionaryId).Wait();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Failure to delete Dictionary", e);
        }
        
    }
}