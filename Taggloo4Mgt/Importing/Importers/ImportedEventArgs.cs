namespace Taggloo4Mgt.Importing.Importers;

/// <summary>
/// Structure to facilitate communication between a consumer and importing tasks.
/// </summary>
public class ImportedEventArgs
{
    /// <summary>
    /// The current item being imported.
    /// </summary>
    public string? CurrentItem { get; set; }
    
    /// <summary>
    /// The IETF Language Tag for the item being imported.
    /// </summary>
    public string? LanguageCode { get; set; }
    
    /// <summary>
    /// Identifies if the message indicates a successful operation.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// The external ID applied to the item being imported.
    /// </summary>
    public string? ExternalId { get; set;  }
    
    /// <summary>
    /// The ID of the item at soutrce.
    /// </summary>
    public int SourceId { get; set; }
}