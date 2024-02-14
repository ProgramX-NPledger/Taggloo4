namespace API.Data.SiteInitialisation.Model;

/// <summary>
/// Represents a Language that must be configured during site initialisation.
/// </summary>
public class Language
{
    /// <summary>
    /// The IETF Language Tag, eg. en-GB
    /// </summary>
    public required string IetfLanguageTag { get; set; }
    
    /// <summary>
    /// The name of the Language.
    /// </summary>
    public required string Name { get; set; }
    
}