using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Taggloo4.Contract.Translation;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;
using Taggloo4.Translation.Translators.Factories;
using Taggloo4.Web.Translation.Translators.Results;

namespace Taggloo4.Translation.Translators;

/// <summary>
/// Performs translations for Words.
/// </summary>
public class WordTranslator : ITranslator
{
    private readonly DataContext _entityFrameworkCoreDatabaseContext;
    private readonly ITranslatorConfiguration _translatorConfiguration;

    /// <summary>
    /// Constructor for configuring the object, called by the <seealso cref="WordTranslatorFactory"/>.
    /// </summary>
    /// <param name="entityFrameworkCoreDatabaseContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <param name="translatorConfiguration">Configuration for Translator.</param>
    public WordTranslator(DataContext entityFrameworkCoreDatabaseContext, ITranslatorConfiguration translatorConfiguration)
    {
        _entityFrameworkCoreDatabaseContext = entityFrameworkCoreDatabaseContext;
        _translatorConfiguration = translatorConfiguration;
    }

    /// <summary>
    /// Performs the translation.
    /// </summary>
    /// <param name="translationRequest">The translation request.</param>
    /// <returns>The results of the translation.</returns>
    public TranslationResults Translate(TranslationRequest translationRequest)
    {
        WordTranslation[] wordTranslations = _entityFrameworkCoreDatabaseContext.WordTranslations
            .Include(m => m.FromWord!.Dictionaries)
            .Include(m => m.ToWord!.Dictionaries)
            .AsNoTracking()
            .Where(q =>
                q.FromWord!.TheWord==translationRequest.Query &&
                q.FromWord!.Dictionaries!.Select(q=>q.IetfLanguageTag).Contains(translationRequest.FromLanguageCode) &&
                q.ToWord!.Dictionaries!.Select(q=>q.IetfLanguageTag).Contains(translationRequest.ToLanguageCode)
            ).OrderBy(q => q.ToWord!.TheWord)
            .Skip(translationRequest.OrdinalOfFirstResult)
            .Take(translationRequest.MaximumNumberOfResults)
            .ToArray();

        // if there were no matches and there were spaces in the query, return null
        if (!wordTranslations.Any() && translationRequest.Query.Contains((" ")))
        {
            return new TranslationResults()
            {
                ResultItems = null
            };
        }

        TranslationResults translationResults = new TranslationResults()
        {
            ResultItems = wordTranslations.Select(q => new WordTranslationResultItem()
            {
                ToWordId = q.ToWordId,
                Translation = q.ToWord?.TheWord ?? "empty",
                FromWordId = q.FromWordId
            })
        };
        
        if (translationRequest.DataWillBePaged)
        {
            // get count of available items to allow paging
            translationResults.NumberOfAvailableItemsBeforePaging = _entityFrameworkCoreDatabaseContext.WordTranslations
                .Include(m => m.FromWord!.Dictionaries)
                .Include(m => m.ToWord!.Dictionaries)
                .AsNoTracking()
                .Count(q => q.FromWord!.TheWord == translationRequest.Query &&
                            q.FromWord!.Dictionaries!.Select(q => q.IetfLanguageTag)
                                .Contains(translationRequest.FromLanguageCode) &&
                            q.ToWord!.Dictionaries!.Select(q => q.IetfLanguageTag)
                                .Contains(translationRequest.ToLanguageCode));
        }

        return translationResults;
    }
    
    /// <inheritdoc cref="ITranslator.Configuration"/>
    public ITranslatorConfiguration Configuration => _translatorConfiguration;

}