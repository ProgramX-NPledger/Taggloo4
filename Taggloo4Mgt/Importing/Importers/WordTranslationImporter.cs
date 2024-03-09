using System.Data.SqlClient;
using Dapper;
using Taggloo4Mgt.Importing.ImportSessions;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.Importers;

public class WordTranslationImporter : IImporter
{
    public string Key { get; } = "wordTranslations";

    public int Ordinal { get; } = 50;
    
    public async Task<IImportSession> CreateSession(SqlConnection sqlConnection)
    {
	    IEnumerable<WordTranslation> wordTranslations = await GetTranslations(sqlConnection);

	    WordTranslationImportSession wordTranslationImportSession = new WordTranslationImportSession(wordTranslations);

	    return wordTranslationImportSession;
    }
    
    

	
    private async Task<IEnumerable<WordTranslation>> GetTranslations(SqlConnection sqlConnection)
    {
        string sqlCmd = @"select fromWord.ID as FromWordId, toT.Translation as TheTranslation, toT.LanguageCode,fromWt.CreatedByUserName,fromWt.CreatedTimeStamp 
							from Taggloo_Word fromWord 
							inner join Taggloo_WordTranslation fromWt on fromWt.WordID=fromWord.ID
							inner join Taggloo_Translation toT on fromWt.TranslationID=toT.ID";
        IEnumerable<WordTranslation> translations = await sqlConnection.QueryAsync<WordTranslation>(sqlCmd);
		
        IEnumerable<WordTranslation> translationsForWord = translations as WordTranslation[] ?? translations.ToArray();
        return translationsForWord;
		
    }
    
}