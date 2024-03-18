using System.Data.SqlClient;
using Dapper;
using Taggloo4Mgt.Importing.ImportSessions;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.Importers;

public class WordImporter : IImporter
{
    public string Key { get; } = "words";
    
    public int Ordinal { get; } = 5;
    
    public async Task<IImportSession> CreateSession(SqlConnection sqlConnection)
    {
        IEnumerable<Word> words = await GetAllWords(sqlConnection);
        
        WordImportSession wordImportSession = new WordImportSession(words);

        return wordImportSession;

    }
    
    private async Task<IEnumerable<Word>> GetAllWords(SqlConnection sqlConnection)
    {
        string sqlCmd =
            "SELECT ID,Word as TheWord, LanguageCode, CreatedTimeStamp, CreatedByUserName, IsBlocked, BlockedByUserName, BlockedTimeStamp FROM Taggloo_Word";

        return await sqlConnection.QueryAsync<Word>(sqlCmd);
    }
    
}