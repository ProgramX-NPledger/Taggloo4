namespace Taggloo4Mgt.Importing.Importers;

public class ImportedEventArgs
{
    public string CurrentItem { get; set; }
    public string LanguageCode { get; set; }
    public bool IsSuccess { get; set; }

    public Guid? ImportGuid { get; set; }

    public int SourceId { get; set; }
}