using API.Data;
using API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace API.Translation.Translators;

public class WordTranslator : ITranslator
{
    private readonly DataContext _entityFrameworkCoreDatabaseContext;

    public WordTranslator(DataContext entityFrameworkCoreDatabaseContext)
    {
        _entityFrameworkCoreDatabaseContext = entityFrameworkCoreDatabaseContext;
    }

    public TranslationResults Translate(TranslationRequest translationRequest)
    {
        WordTranslation[] wordTranslations = _entityFrameworkCoreDatabaseContext.WordTranslations
            .Include(m => m.FromWord!.Dictionary)
            .Include(m => m.ToWord!.Dictionary)
            .AsNoTracking()
            .Where(q =>
                q.FromWord!.TheWord==translationRequest.Query &&
                q.FromWord!.Dictionary!.IetfLanguageTag==translationRequest.FromLanguageCode &&
                q.ToWord!.Dictionary!.IetfLanguageTag==translationRequest.ToLanguageCode
            ).OrderBy(q => q.ToWord!.TheWord)
            .Skip(translationRequest.OrdinalOfFirstResult)
            .Take(translationRequest.MaximumNumberOfResults)
            .ToArray();
        
        return new TranslationResults()
        {
            ResultItems = wordTranslations.Select(q=>new
            {
                Translation=q.ToWord?.TheWord,
                ToWordId=q.ToWordId
            })
        };
    }
}