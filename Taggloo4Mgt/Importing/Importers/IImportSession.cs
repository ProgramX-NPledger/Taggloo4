namespace Taggloo4Mgt.Importing.Importers;

public interface IImportSession
{
    int GetToBeImportedCount();
    

    Task ImportAcrossDictionariesAsync(HttpClient httpClient,
        string languageCode1,
        int dictionary1Id,
        string languageCode2,
        int dictionary2Id);
    
    event EventHandler<ImportEventArgs>? LogMessage;
    event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
    event EventHandler<ImportedEventArgs>? Imported;
}