namespace Taggloo4.Model;

/// <summary>
/// Represents a Dictionary including Content Type and Language.
/// </summary>
public class DictionaryWithContentTypeAndLanguage
{
    /// <summary>
    /// Identifier of Dictionary.
    /// </summary>
    public int DictionaryId { get; set; }
    
    /// <summary>
    /// Name of Dictionary.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Description of Dictionary.
    /// </summary>
    public required string Description { get; set; }
    
    /// <summary>
    /// Timestamp of creation.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Creator of Dictionary.
    /// </summary>
    public string? CreatedByUserName { get; set; }
    
    /// <summary>
    /// Host on which Dictionary was created.
    /// </summary>
    public required string CreatedOn { get; set; }
    
    /// <summary>
    /// URL referring to the source data.
    /// </summary>
    public required string SourceUrl { get; set; }
    
    /// <summary>
    /// Identifier of Content Type of Dictionary.
    /// </summary>
    public int ContentTypeId { get; set; }
    
    /// <summary>
    /// Disambiguating key for Content Type.
    /// </summary>
    public required string ContentTypeKey { get; set; }
    
    /// <summary>
    /// Controller for Content Type.
    /// </summary>
    public required string Controller { get; set; }
    
    /// <summary>
    /// Plural name for Content Type.
    /// </summary>
    public required string NamePlural { get; set; }
    
    /// <summary>
    /// Singular name for Content Type.
    /// </summary>
    public required string NameSingular { get; set; }
    
    /// <summary>
    /// IETF Language Tag.
    /// </summary>
    public required string IetfLanguageTag { get; set; }
    
    /// <summary>
    /// Name of Language within Dictionary.
    /// </summary>
    public required string LanguageName { get; set; }
}