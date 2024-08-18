using API.Data;

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
        return new TranslationResults();
    }
}