namespace Taggloo4.Web.Areas.Admin.Models.DTOs;

/// <summary>
/// Represents a Language within Taggloo.
/// </summary>
public class LanguageDto
{
    /// <summary>
    /// IETF Language Tag to refer to the Language.
    /// </summary>
    public required string IetfLanguageTag { get; set; }
    
    /// <summary>
    /// Name of the Language.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Number of Dictionaries that this Language appears in.
    /// </summary>
    public required int DictionariesCount { get; set; }
    
}