using System.Data.SqlClient;
using Dapper;
using Taggloo4Mgt.Importing.ImportSessions;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.Importers;

public class PhraseTranslationImporter : IImporter
{
    public string Key { get; } = "phraseTranslations";
    
    public int Ordinal { get; } = 60;
    
    public async Task<IImportSession> CreateSession(SqlConnection sqlConnection)
    {
	    IEnumerable<PhraseTranslation> phraseTranslations = await GetTranslations(sqlConnection);

	    PhraseTranslationImportSession phraseTranslationImportSession = new PhraseTranslationImportSession(phraseTranslations);

	    return phraseTranslationImportSession;
    }
    
    
	
    private async Task<IEnumerable<PhraseTranslation>> GetTranslations(SqlConnection sqlConnection)
    {
	    string sqlCmd = @"select t.ID,pt.PhraseID as FromPhraseId, t.ID as ToPhraseID, t.Translation,t.LanguageCode,pt.CreatedByUserName,pt.CreatedTimeStamp from Taggloo_Phrase p
			inner join Taggloo_PhraseTranslation pt on p.id=pt.PhraseID
			inner join Taggloo_Translation t on t.ID=pt.TranslationID";

	    IEnumerable<PhraseTranslation> translations = await sqlConnection.QueryAsync<PhraseTranslation>(sqlCmd);
		
	    // make sure all the returned languages are supported
	    IEnumerable<PhraseTranslation> translationsForPhrase = translations as PhraseTranslation[] ?? translations.ToArray();
	    return translationsForPhrase;
		
    }

    
    
    
}