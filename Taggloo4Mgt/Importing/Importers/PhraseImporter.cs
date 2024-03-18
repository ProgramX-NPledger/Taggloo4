using System.Data.SqlClient;
using Dapper;
using Taggloo4Mgt.Importing.ImportSessions;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.Importers;

public class PhraseImporter : IImporter
{
    public string Key { get; } = "phrases";
    
    public int Ordinal { get; } = 10;
    
    public async Task<IImportSession> CreateSession(SqlConnection sqlConnection)
    {
        IEnumerable<Phrase> phrases = await GetAllPhrases(sqlConnection);
        
        PhraseImportSession phraseImportSession = new PhraseImportSession(phrases);

        return phraseImportSession;
        
    }
    
    
	

    private async Task<IEnumerable<Phrase>> GetAllPhrases(SqlConnection sqlConnection)
    {
        string sqlCmd =
            @"SELECT ID,Phrase as ThePhrase,LanguageCode,CreatedTimeStamp,CreatedByUserName, DictionaryID FROM Taggloo_Phrase";

        return await sqlConnection.QueryAsync<Phrase>(sqlCmd);
    }

    
}