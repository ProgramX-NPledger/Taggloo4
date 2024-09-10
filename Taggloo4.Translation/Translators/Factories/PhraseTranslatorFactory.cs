using Taggloo4.Contract.Translation;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Web.Translation.Translators;

namespace Taggloo4.Translation.Translators.Factories;

/// <summary>
/// Instantiates and configures <seealso cref="PhraseTranslator"/> instance.
/// </summary>

public class PhraseTranslatorFactory : ITranslatorFactory
{
    /// <summary>
    /// Instantiates and configures a <seealso cref="PhraseTranslator"/> instance.
    /// </summary>
    /// <param name="entityFrameworkCoreDatabaseContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <param name="translatorConfiguration">Configuration for Translator.</param>
    /// <returns>A configured <seealso cref="PhraseTranslator"/> as an implementation of <seealso cref="ITranslator"/>.</returns>
    public ITranslator Create(DataContext entityFrameworkCoreDatabaseContext, ITranslatorConfiguration translatorConfiguration)
    {
        return new PhraseTranslator(entityFrameworkCoreDatabaseContext, translatorConfiguration);
    }
    
    
    /// <inheritdoc cref="ITranslatorFactory"/>
    public string GetTranslatorName()
    {
        return nameof(PhraseTranslator);
    }

}