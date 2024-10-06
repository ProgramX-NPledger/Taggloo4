namespace Taggloo4.Model.Exceptions;

public class DictionariesSummary
{
    /// <summary>
    /// Number of Languages in Dictionaries
    /// </summary>
    public int NumberOfLanguagesInDictionaries { get; set; }
    
    /// <summary>
    /// Number of Dictionaries.
    /// </summary>
    public int NumberOfDictionaries { get; set; }
    
    /// <summary>
    /// Number of Content Types in Dictionaries.
    /// </summary>
    /// <remarks>
    /// There may be more Content Types available that are being used in Dictionaries.
    /// </remarks>
    public int NumberOfContentTypes { get; set; }
}