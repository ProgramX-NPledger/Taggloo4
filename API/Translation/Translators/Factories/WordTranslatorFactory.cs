using System.Composition;
using API.Data;

namespace API.Translation.Translators.Factories;

/// <summary>
/// Instantiates and configures <seealso cref="WordTranslator"/> instance.
/// </summary>

public class WordTranslatorFactory : ITranslatorFactory
{
    /// <summary>
    /// Instantiates and configures a <seealso cref="WordTranslator"/> instance.
    /// </summary>
    /// <param name="entityFrameworkCoreDatabaseContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <returns>A configured <seealso cref="WordTranslator"/> as an implementation of <seealso cref="ITranslator"/>.</returns>
    public ITranslator Create(DataContext entityFrameworkCoreDatabaseContext)
    {
        return new WordTranslator(entityFrameworkCoreDatabaseContext);
    }
    
    
    /// <inheritdoc cref="ITranslatorFactory"/>
    public string GetTranslatorName()
    {
        return nameof(WordTranslator);
    }

    
}