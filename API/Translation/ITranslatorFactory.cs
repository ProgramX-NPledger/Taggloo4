using API.Data;

namespace API.Translation;

public interface ITranslatorFactory
{
    ITranslator Create(DataContext entityFrameworkCoreDatabaseContext);
}