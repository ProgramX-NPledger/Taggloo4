using System.Collections;
using System.Reflection;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Translation;

public class ContentTypeManagerFactory
{
    /// <summary>
    /// Create a <seealso cref="IContentTypeManager"/> capable of managing a Dictionary.
    /// </summary>
    /// <param name="dictionary">The Dictionary to create a manager for.</param>
    /// <returns>An implementation of <seealso cref="IDictionaryManager"/></returns>
    public static IContentTypeManager CreateContentTypeManager(DataContext dataContext, Dictionary dictionary)
    {
        if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
        if (dictionary.ContentType==null) throw new InvalidOperationException($"Dictionary ID {dictionary.Id} has no {nameof(dictionary.ContentType)}");
        
        if (string.IsNullOrEmpty(dictionary.ContentType.ContentTypeManagerDotNetAssemblyName))
            throw new InvalidOperationException(
                $"{nameof(dictionary.ContentType.ContentTypeManagerDotNetAssemblyName)} cannot be null or empty.");
        if (string.IsNullOrEmpty(dictionary.ContentType.ContentTypeManagerDotNetTypeName))
            throw new InvalidOperationException(
                $"{nameof(dictionary.ContentType.ContentTypeManagerDotNetTypeName)} cannot be null or empty.");
        
        // load assembly
        Assembly assembly = Assembly.Load(dictionary.ContentType.ContentTypeManagerDotNetAssemblyName);
        if (assembly is null) throw new InvalidOperationException($"Failed to load assembly '{dictionary.ContentType.ContentTypeManagerDotNetAssemblyName}'.");
        
        // load type
        Type? type = assembly.GetType(dictionary.ContentType.ContentTypeManagerDotNetTypeName);
        if (type is null) throw new InvalidOperationException($"Failed to load type '{dictionary.ContentType.ContentTypeManagerDotNetTypeName}'.");
        
        // instantiate
        object? instance = Activator.CreateInstance(type);
        if (instance is null) throw new InvalidOperationException($"Failed to create instance of type '{type}'.");

        if (instance is IContentTypeManager)
        {
            IContentTypeManager contentTypeManager = (IContentTypeManager)instance;
            _ = contentTypeManager.Initialise(dataContext, dictionary);
            
            return contentTypeManager;
        }
        
        throw new InvalidOperationException($"Type '{type}' does not implement {nameof(IContentTypeManager)}");
    }
}