using Taggloo4Mgt.Importing.Importers;

namespace Taggloo4Mgt.Importing.ImportSessions;

public class PhraseImportSession : IImportSession
{
    public int GetToBeImportedCount()
    {
        throw new NotImplementedException();
    }

    public Task Import(HttpClient httpClient, string languageCode, int dictionaryId)
    {
        throw new NotImplementedException();
    }

    public event EventHandler<ImportEventArgs>? LogMessage;
    public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
    public event EventHandler<ImportedEventArgs>? Imported;
}