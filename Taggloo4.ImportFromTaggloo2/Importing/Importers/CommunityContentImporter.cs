using System.Data.SqlClient;
using Dapper;
using Taggloo4Mgt.Importing.ImportSessions;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.Importers;

public class CommunityContentImporter : IImporter
{
    public string Key { get; } = "communitycontent";
    
    public int Ordinal { get; } = 10;
    
    public async Task<IImportSession> CreateSession(SqlConnection sqlConnection)
    {
        IEnumerable<CommunityContentItem> communityContentItems = await GetAllCommunityContentItems(sqlConnection);
        
        CommunityContentImportSession communityContentImportSession = new CommunityContentImportSession(communityContentItems);

        return communityContentImportSession;
        
    }
    
    
	

    private async Task<IEnumerable<CommunityContentItem>> GetAllCommunityContentItems(SqlConnection sqlConnection)
    {
        string sqlCmd =
            @"SELECT cci.ID,Title,SynopsisText,ImageUrl,AuthorName,AuthorUrl,SourceUrl,PublishedTimeStamp,Hash,OriginalSynopsisHtml,IsTruncated,RetrievedTimeStamp,LanguageCode,
	ccd.ID as DiscovererID,ccd.DotNetTypeName,ccd.IsEnabled,ccd.Name
	FROM Taggloo_CommunityContentItem cci
	INNER JOIN Taggloo_CC_Discoverer ccd on ccd.ID=cci.CC_DiscovererID
";

        return await sqlConnection.QueryAsync<CommunityContentItem>(sqlCmd);
    }

    
}