using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;
using Taggloo4.Translation;

namespace Taggloo4.Web.Hangfire.Jobs;

/// <summary>
/// Job to delete a Dictionary and all included content.
/// </summary>
public class DeleteDictionaryJob
{
    private readonly IDictionaryRepository _dictionaryRepository;
    private readonly DataContext _dataContext;

    /// <summary>
    /// Constructor with injected parameters.
    /// </summary>
    /// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
    /// <param name="dataContext">Entity Framework Data Context providing full access to the datastore without requiring a
    /// Repository interface.</param>
    public DeleteDictionaryJob(IDictionaryRepository dictionaryRepository, DataContext dataContext)
    {
        _dictionaryRepository = dictionaryRepository;
        _dataContext = dataContext;
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
            // get the Content Type Manager for the Dictionary
            IContentTypeManager contentTypeManager = ContentTypeManagerFactory.CreateContentTypeManager(_dataContext,dictionary);
            
            // have the Content Type Manager delete the Dictionary
            contentTypeManager.DeleteDictionaryAndContentsAsync().Wait();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Failure to delete Dictionary", e);
        }
        
    }
}