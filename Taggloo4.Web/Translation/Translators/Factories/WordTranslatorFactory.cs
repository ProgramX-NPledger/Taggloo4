using System.Composition;
using System.Configuration;
using Taggloo4.Contract;
using Taggloo4.Contract.Translation;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Data;

namespace Taggloo4.Web.Translation.Translators.Factories;

/// <summary>
/// Instantiates and configures <seealso cref="WordTranslator"/> instance.
/// </summary>

public class WordTranslatorFactory : ITranslatorFactory
{
    /// <summary>
    /// Instantiates and configures a <seealso cref="WordTranslator"/> instance.
    /// </summary>
    /// <param name="translatorConfiguration">Configuration for Translator.</param>
    /// <param name="entityFrameworkCoreDatabaseContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <returns>A configured <seealso cref="WordTranslator"/> as an implementation of <seealso cref="ITranslator"/>.</returns>
    public ITranslator Create(DataContext entityFrameworkCoreDatabaseContext, ITranslatorConfiguration translatorConfiguration)
    {
        return new WordTranslator(entityFrameworkCoreDatabaseContext,translatorConfiguration);
    }
    
    
    /// <inheritdoc cref="ITranslatorFactory.GetTranslatorName"/>
    public string GetTranslatorName()
    {
        return nameof(WordTranslator);
    }

    /// <inheritdoc cref="System.Configuration.Configuration"/>
    public ITranslatorConfiguration Configuration { get; set; }
}