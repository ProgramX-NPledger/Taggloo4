using System.Data.SqlClient;
using Dapper;
using Taggloo4Mgt.Importing.ImportSessions;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.Importers;

public class TranslatedPhraseImporter : IImporter
{
    public string Key { get; } = "translatedPhraseTranslations";
    
    public int Ordinal { get; } = 50;
    
    public async Task<IImportSession> CreateSession(SqlConnection sqlConnection)
    {
	    IEnumerable<Phrase> phraseTranslations = await GetTranslations(sqlConnection);

	    TranslatedPhraseImportSession translatedPhraseImportSession = new TranslatedPhraseImportSession(phraseTranslations);

	    return translatedPhraseImportSession;
    }
    
    
	
    private async Task<IEnumerable<Phrase>> GetTranslations(SqlConnection sqlConnection)
    {
	    string sqlCmd = @"select t.id,Translation as ThePhrase,LanguageCode,pt.CreatedTimeStamp,pt.CreatedByUserName,pt.DictionaryID
	from Taggloo_Translation t
	inner join Taggloo_PhraseTranslation pt on pt.TranslationID=t.id
	where TypeEncoded='Phrase'";

	    IEnumerable<Phrase> phrases = await sqlConnection.QueryAsync<Phrase>(sqlCmd);
		
	    // make sure all the returned languages are supported
	    IEnumerable<Phrase> translationsForPhrase = phrases as Phrase[] ?? phrases.ToArray();
	    return translationsForPhrase;
		
    }

    
    
    
}