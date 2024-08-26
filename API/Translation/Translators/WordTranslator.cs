﻿using API.Data;
using API.Model;
using API.Translation.Translators.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace API.Translation.Translators;

/// <summary>
/// Performs translations for Words.
/// </summary>
public class WordTranslator : ITranslator
{
    private readonly DataContext _entityFrameworkCoreDatabaseContext;

    /// <summary>
    /// Constructor for configuring the object, called by the <seealso cref="WordTranslatorFactory"/>.
    /// </summary>
    /// <param name="entityFrameworkCoreDatabaseContext">Entity Framework context to enable access to underlying datastore.</param>
    public WordTranslator(DataContext entityFrameworkCoreDatabaseContext)
    {
        _entityFrameworkCoreDatabaseContext = entityFrameworkCoreDatabaseContext;
    }

    /// <summary>
    /// Performs the translation.
    /// </summary>
    /// <param name="translationRequest">The translation request.</param>
    /// <returns>The results of the translation.</returns>
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
                .Include(m => m.FromWord!.Dictionary)
                .Include(m => m.ToWord!.Dictionary)
                .AsNoTracking()
                .Count(q => q.FromWord!.TheWord==translationRequest.Query &&
                                           q.FromWord!.Dictionary!.IetfLanguageTag==translationRequest.FromLanguageCode &&
                                           q.ToWord!.Dictionary!.IetfLanguageTag==translationRequest.ToLanguageCode);
        }

        return translationResults;
    }
}