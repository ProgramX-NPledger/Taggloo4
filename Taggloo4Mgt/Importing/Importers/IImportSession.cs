namespace Taggloo4Mgt.Importing.Importers;

public interface IImportSession
{
    int GetToBeImportedCount();

    Task Import(HttpClient httpClient, string languageCode, int dictionaryId, Dictionary<string, Dictionary<int, string>> originalIdsToImportIdsMap);

    event EventHandler<ImportEventArgs>? LogMessage;
    event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
    event EventHandler<ImportedEventArgs>? Imported;
}