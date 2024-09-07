using Taggloo4.Contract;
using Taggloo4.Model;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Translation;

/// <summary>
/// Managed as a singleton, provides access to commonly used configuration data for Translators.
/// </summary>
public class TranslatorConfigurationCache
{
    private IDictionary<string, ITranslatorConfiguration> _translatorConfigurationDictionary;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public TranslatorConfigurationCache()
    {
        _translatorConfigurationDictionary = new Dictionary<string, ITranslatorConfiguration>();
    }
    
    /// <summary>
    /// Gets the configuration for a Translator. If there is a configuration record for a Translator, this is returned, otherwise
    /// a <seealso cref="DefaultTranslatorConfiguration"/> is returned.
    /// </summary>
    /// <param name="translatorKey">Key representing the Translator.</param>
    /// <param name="translatorConfigurationRepository">An implementation of a <seealso cref="ITranslatorConfigurationRepository"/>.</param>
    /// <returns>The requested configuration or a default configuration.</returns>
    /// <exception cref="ArgumentNullException">Thrown if a required argument is not provided.</exception>
    /// <remarks>
    /// Returned configurations are cached. This object is handled as a singleton to facilitate more efficient retrieval of commonly
    /// used data.
    /// </remarks>
    public async Task<ITranslatorConfiguration> GetTranslatorConfiguration(string translatorKey,
        ITranslatorConfigurationRepository translatorConfigurationRepository)
    {
        if (translatorKey == null) throw new ArgumentNullException(nameof(translatorKey));
        if (translatorConfigurationRepository == null)
            throw new ArgumentNullException(nameof(translatorConfigurationRepository));
        
        // look in cache to see if have configuration
        if (_translatorConfigurationDictionary.ContainsKey(translatorKey))
            return _translatorConfigurationDictionary[translatorKey];
        
        // no cached configuration, look for it in DB
        TranslatorConfiguration? translatorConfiguration = await translatorConfigurationRepository.GetTranslatorConfiguration(translatorKey);
        ITranslatorConfiguration resolvedTranslatorConfiguration;
        if (translatorConfiguration == null)
        {
            // no configuraation, create default
            resolvedTranslatorConfiguration = new DefaultTranslatorConfiguration();
        }
        else
        {
            resolvedTranslatorConfiguration = translatorConfiguration;
        }
        
        // add to cache
        _translatorConfigurationDictionary.Add(translatorKey, resolvedTranslatorConfiguration);

        return resolvedTranslatorConfiguration;
    }
}