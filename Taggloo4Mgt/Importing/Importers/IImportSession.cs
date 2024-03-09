namespace Taggloo4Mgt.Importing.Importers;

public interface IImportSession
{
    int GetToBeImportedCount();

    Task Import(HttpClient httpClient, string languageCode, int dictionaryId);

    event EventHandler<ImportEventArgs>? LogMessage;
    event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
    event EventHandler<ImportedEventArgs>? Imported;
}