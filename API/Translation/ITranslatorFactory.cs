using API.Contract;
using API.Data;
using API.Model;

namespace API.Translation;

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