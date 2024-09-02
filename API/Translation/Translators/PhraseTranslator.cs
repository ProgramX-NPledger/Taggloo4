using API.Data;
using API.Model;
using API.Translation.Translators.Factories;
using API.Translation.Translators.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace API.Translation.Translators;

/// <summary>
/// Performs translations for Phrases.
/// </summary>
public class PhraseTranslator : ITranslator
{
    private readonly DataContext _entityFrameworkCoreDatabaseContext;

    /// <summary>
    /// Constructor for configuring the object, called by the <seealso cref="WordTranslatorFactory"/>.
    /// </summary>
    /// <param name="entityFrameworkCoreDatabaseContext">Entity Framework context to enable access to underlying datastore.</param>
    public PhraseTranslator(DataContext entityFrameworkCoreDatabaseContext)
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
        // break apart the words and look for the words in phrases, returning a % of matches
        string[] individualWords = translationRequest.Query.Trim().Split(new char[] { ' ' }).Where(q=>!string.IsNullOrWhiteSpace(q)).ToArray();
        // we first get all the phrases the word appears in and use that as a source list to be able to calculate % matches
        // therefore relying on database indexing/efficiencies before working with a smaller dataset.
        List<Phrase> untranslatedWordAppearsInPhrases = new List<Phrase>();
        foreach (string wordToMatch in individualWords)
        {
            WordInPhrase[] untranslatedWordInPhrases = _entityFrameworkCoreDatabaseContext.WordsInPhrases
                .Include(m => m.Word.Dictionary)
                .Include(m => m.InPhrase.Dictionary)
                .AsNoTracking()
                .Where(q => q.Word.TheWord == wordToMatch &&
                            q.Word.Dictionary!.IetfLanguageTag == translationRequest.FromLanguageCode &&
                            q.InPhrase.Dictionary!.IetfLanguageTag == translationRequest.FromLanguageCode)
                .ToArray();
            // add to collection of phrases the word appears in (we're processing in phrases not words)
            untranslatedWordAppearsInPhrases.AddRange(untranslatedWordInPhrases.Select(q=>q.InPhrase));
        }
        // group the untranslated appearances of the word by the phrase they appear in
        // (multiple instances of the same phrase will have been returned by the above multiple words appear in the same phrase)
        IEnumerable<IGrouping<int,Phrase>> untranslatedPhrases = untranslatedWordAppearsInPhrases.GroupBy(g => g.Id);

        // translate the phrases
        List<PhraseTranslationResultItem> phraseTranslationResultItems = new List<PhraseTranslationResultItem>();
        foreach (IGrouping<int, Phrase> groupedUntranslatedPhrase in untranslatedPhrases)
        {
            Phrase untranslatedPhrase = groupedUntranslatedPhrase.First();

            PhraseTranslation[] phraseTranslations = _entityFrameworkCoreDatabaseContext.PhraseTranslations
                .Include(m=>m.ToPhrase)
                .Include(m=>m.FromPhrase)
                .AsNoTracking()
                .Where(q => q.FromPhraseId == untranslatedPhrase.Id)
                .ToArray();
            
            // add the translated phrase to the results
            foreach (PhraseTranslation phraseTranslation in phraseTranslations)
            {
                PhraseTranslationResultItem phraseTranslationResultItem = new PhraseTranslationResultItem()
                {
                    FromPhraseId = phraseTranslation.FromPhraseId,
                    ToPhraseId = phraseTranslation.ToPhraseId,
                    Translation = phraseTranslation.ToPhrase!.ThePhrase,
                    FromPhrase = phraseTranslation.FromPhrase?.ThePhrase, // this is null
                    PercentageMatch = 100 // TODO
                };
                
                phraseTranslationResultItems.Add(phraseTranslationResultItem);
            }

            
        }

        TranslationResults translationResults;
        if (translationRequest.DataWillBePaged)
        {
            translationResults = new TranslationResults()
            {
                ResultItems = phraseTranslationResultItems.Skip(translationRequest.OrdinalOfFirstResult).Take(translationRequest.MaximumNumberOfResults),
                NumberOfAvailableItemsBeforePaging = phraseTranslationResultItems.Count()
            };
        }
        else
        {
            translationResults = new TranslationResults()
            {
                ResultItems = phraseTranslationResultItems
            };
        }

        return translationResults;
    }
}