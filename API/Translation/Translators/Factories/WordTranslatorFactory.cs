using API.Data;

namespace API.Translation.Translators.Factories;

public class WordTranslatorFactory : ITranslatorFactory
{
    public ITranslator Create(DataContext entityFrameworkCoreDatabaseContext)
    {
        return new WordTranslator(entityFrameworkCoreDatabaseContext);
    }
}