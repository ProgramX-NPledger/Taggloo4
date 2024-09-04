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
    /// <returns>Implementation of <seealso cref="ITranslator"/> for servicing translation results.</returns>
    ITranslator Create(DataContext entityFrameworkCoreDatabaseContext);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    string GetTranslatorName();

    /// <summary>
    /// Configuration of the Translator.
    /// </summary>
    ITranslatorConfiguration Configuration { get; set; }
    
}