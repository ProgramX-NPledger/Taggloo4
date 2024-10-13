using System.Collections;
using System.Reflection;
using Taggloo4.Contract;
using Taggloo4.Model;

namespace Taggloo4.Translation;

public class DictionaryManagerFactory
{
    /// <summary>
    /// Create a <seealso cref="IDictionaryFactory"/> capable of managing a Dictionary.
    /// </summary>
    /// <param name="dictionary">The Dictionary to create a manager for.</param>
    /// <returns>An implementation of <seealso cref="IDictionaryManager"/></returns>
    public static IDictionaryManager CreateDictionaryManager(Dictionary dictionary)
    {
        if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
        if (string.IsNullOrEmpty(dictionary.DictionaryManagerDotNetAssemblyName))
            throw new InvalidOperationException(
                $"{nameof(dictionary.DictionaryManagerDotNetAssemblyName)} cannot be null or empty.");
        if (string.IsNullOrEmpty(dictionary.DictionaryManagerDotNetTypeName))
            throw new InvalidOperationException(
                $"{nameof(dictionary.DictionaryManagerDotNetTypeName)} cannot be null or empty.");
        
        // load assembly
        Assembly assembly = Assembly.Load(dictionary.DictionaryManagerDotNetAssemblyName);
        if (assembly is null) throw new InvalidOperationException($"Failed to load assembly '{dictionary.DictionaryManagerDotNetAssemblyName}'.");
        
        // load type
        Type? type = assembly.GetType(dictionary.DictionaryManagerDotNetTypeName);
        if (type is null) throw new InvalidOperationException($"Failed to load type '{dictionary.DictionaryManagerDotNetTypeName}'.");
        
        // instatiate
        object? instance = Activator.CreateInstance(type);
        if (instance is null) throw new InvalidOperationException($"Failed to create instance of type '{type}'.");

        if (instance is IDictionaryManager)
        {
            IDictionaryManager dictionaryManager = (IDictionaryManager)instance;
            return dictionaryManager;
        }
        
        throw new InvalidOperationException($"Type '{type}' does not implement {nameof(IDictionaryManager)}");
    }
}