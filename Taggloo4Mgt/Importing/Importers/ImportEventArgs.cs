namespace Taggloo4Mgt.Importing.Importers;

/// <summary>
/// Event arguments to allow transfer of data to ocnsumer for logigng purposes.
/// </summary>
public class ImportEventArgs : EventArgs 
{
    /// <summary>
    /// Message to include in logs.
    /// </summary>
    public string LogMessage { get; set; }
    
    /// <summary>
    /// Number of indentations for text.
    /// </summary>
    public int Indentation { get; set; }
    
}