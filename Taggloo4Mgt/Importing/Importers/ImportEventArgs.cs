namespace Taggloo4Mgt.Importing.Importers;

public class ImportEventArgs : EventArgs 
{
    public string LogMessage { get; set; }
    public int Indentation { get; set; }
    public int? MillisecondsBetweenWords { get; set; }
}