using System.Composition;
using API.Contract;
using API.Data;

namespace API.Translation.Translators.Factories;

/// <summary>
/// Instantiates and configures <seealso cref="PhraseTranslator"/> instance.
/// </summary>

public class PhraseTranslatorFactory : ITranslatorFactory
{
    /// <summary>
    /// Instantiates and configures a <seealso cref="PhraseTranslator"/> instance.
    /// </summary>
    /// <param name="entityFrameworkCoreDatabaseContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <returns>A configured <seealso cref="PhraseTranslator"/> as an implementation of <seealso cref="ITranslator"/>.</returns>
    public ITranslator Create(DataContext entityFrameworkCoreDatabaseContext)
    {
        return new PhraseTranslator(entityFrameworkCoreDatabaseContext);
    }
    
    
    /// <inheritdoc cref="ITranslatorFactory"/>
    public string GetTranslatorName()
    {
        return nameof(PhraseTranslator);
    }

    /// <inheritdoc cref="ITranslatorFactory.Configuration"/>
    public ITranslatorConfiguration Configuration { get; set; }
}