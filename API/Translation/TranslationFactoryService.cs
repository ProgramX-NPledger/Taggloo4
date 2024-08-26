using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;
using API.Contract;
using API.Model;
using NuGet.Protocol.Plugins;

namespace API.Translation;

/// <summary>
/// Provides services for <seealso cref="ITranslatorFactory"/> items which may be used to provide translations.
/// </summary>
public class TranslationFactoryService
{
    private CompositionHost? _compositionHost;
    
    /// <summary>
    /// Called as part of application initialisation, configures the internal MEF-graph.
    /// </summary>
    internal void Initialise()
    {
        // build MEF-based graph
        var conventions = new ConventionBuilder();
        conventions
            .ForTypesDerivedFrom<ITranslatorFactory>()
            .Export<ITranslatorFactory>()
            .Shared();
 
        Assembly[] assemblies = new[] { typeof(Program).Assembly };
 
        ContainerConfiguration containerConfiguration = new ContainerConfiguration()
            .WithAssemblies(assemblies, conventions);

        _compositionHost = containerConfiguration.CreateContainer();

    }

    /// <summary>
    /// Returns a filtered list of <seealso cref="ITranslatorFactory"/> instances matching the provided filter. 
    /// </summary>
    /// <param name="translatorRepository">An implementation of the <seealso cref="ITranslatorRepository"/> that
    /// may be used to access configurational data for the <seealso cref="ITranslatorFactory"/>.</param>
    /// <param name="isEnabled">If specified, whether the <seealso cref="ITranslatorFactory"/> should be enabled.</param>
    /// <returns>A list of matching instances of <seealso cref="ITranslatorFactory"/> which may be used to provide
    /// translations.</returns>
    public async Task<IEnumerable<ITranslatorFactory>> GetTranslatorFactoriesAsync(ITranslatorRepository translatorRepository, bool? isEnabled)
    {
        if (_compositionHost == null) throw new InvalidOperationException($"{nameof(_compositionHost)} cannot be null");
        
        IEnumerable<ITranslatorFactory>? translatorFactories =_compositionHost.GetExports<ITranslatorFactory>();
        
        // pass some filters to teh repository and return their configurations
        Translator[] translatorFactoryEntities=(await translatorRepository.GetAllTranslatorsAsync(null, isEnabled)).ToArray();

        List<ITranslatorFactory> matchingTranslatorFactories = new List<ITranslatorFactory>();
        foreach (ITranslatorFactory translatorFactory in translatorFactories)
        {
            if (translatorFactoryEntities.Any(q=>q.Key.Equals(translatorFactory.GetTranslatorName(),StringComparison.OrdinalIgnoreCase)))
                matchingTranslatorFactories.Add(translatorFactory);
        }
        
        return matchingTranslatorFactories;
    }
    
    
}