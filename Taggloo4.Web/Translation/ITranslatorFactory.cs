using Taggloo4.Contract;
using Taggloo4.Contract.Translation;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Web.Model;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Data;

namespace Taggloo4.Web.Translation;

/// <summary>
/// Instantiates and configures Translators.
/// </summary>
public interface ITranslatorFactory
{
    /// <summary>
    /// Instantiate and configure the Translator.
    /// </summary>
    /// <param name="entityFrameworkCoreDatabaseContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <param name="translatorConfiguration">Configuration for Translator.</param>
    /// <returns>Implementation of <seealso cref="ITranslator"/> for servicing translation results.</returns>
    ITranslator Create(DataContext entityFrameworkCoreDatabaseContext, ITranslatorConfiguration translatorConfiguration);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    string GetTranslatorName();

 
    
}