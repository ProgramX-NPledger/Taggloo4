namespace Taggloo4.Web.Areas.Admin.ViewModels.Words;

/// <summary>
/// Dictionary View Model.
/// </summary>
public class DictionaryViewModel
{
    /// <summary>
    /// Identifier of Dictionary.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of Dictionary.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// IETF Language Tag of Language of Dictionary.
    /// </summary>
    public string IetfLanguageTag { get; set; }
    
    /// <summary>
    /// Name of Language of Dictionary.
    /// </summary>
    public string LanguageName { get; set; }
    
}