
using System.Net.Http.Json;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;
using Taggloo4Mgt.Importing.Importers;
using Taggloo4Mgt.Importing.Model;

namespace Taggloo4Mgt.Importing.ImportSessions;

public class CommunityContentImportSession : IImportSession
{
    private readonly IEnumerable<CommunityContentItem> _communityContentItems;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CommunityContentImportSession(IEnumerable<CommunityContentItem> communityContentItem)
    {
        _communityContentItems = communityContentItem;
    }
    
    public int GetToBeImportedCount()
    {
        return _communityContentItems.Count();
    }

    public string ContentTypeKey { get; } = "CommunityContentItem";


    public event EventHandler<ImportEventArgs>? LogMessage;
    public event EventHandler<ImportMetricsEventArgs>? UpdateMetrics;
    public event EventHandler<ImportedEventArgs>? Imported;
    
    public async Task ImportAcrossDictionariesAsync(HttpClient httpClient, string languageCode1, int dictionary1Id, string languageCode2,
	    int dictionary2Id)
    {

	    await ImportWithinDictionaryAsync(httpClient, languageCode1, dictionary1Id);
	    await ImportWithinDictionaryAsync(httpClient, languageCode2, dictionary2Id);
    }
    
    
    private async Task ImportWithinDictionaryAsync(HttpClient httpClient, string languageCode, int dictionaryId)
    {
	    CommunityContentItem[] communityContentItemInLanguage =
        		    _communityContentItems.Where(q => q.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase)).ToArray();
        
        LogMessage?.Invoke(this,new ImportEventArgs()
        {
        	LogMessage = $"Importing {communityContentItemInLanguage.Count()} Phrases for Language {languageCode}",
        	Indentation = 4
        });
        
        foreach (CommunityContentItem communityContentItem in communityContentItemInLanguage)
        {
        	DateTime startTimeStamp = DateTime.Now;
        	
        	LogMessage?.Invoke(this,new ImportEventArgs()
        	{
        		LogMessage = $"{communityContentItem.Title}",
        		Indentation = 5
        	});

        	try
        	{
		        _ = await PostCommunityContentItemToTarget(httpClient, communityContentItem, dictionaryId);
		        Imported?.Invoke(this,new ImportedEventArgs()
		        {
			        LanguageCode = languageCode,
			        CurrentItem = communityContentItem.Title,
			        IsSuccess = true,
			        SourceId = communityContentItem.ID
		        });
        	}
        	catch (Exception ex)
        	{
        		await Console.Error.WriteLineAsync();
        		await Console.Error.WriteLineAsync($"Failed to import Community Content Item '{communityContentItem.Title}'");
        		Exception? exPtr = ex;
        		do
        		{
        			await Console.Error.WriteLineAsync(exPtr.Message);
        			LogMessage?.Invoke(this, new ImportEventArgs()
        			{
        				LogMessage = $"ERROR: {exPtr.GetType().Name}: {exPtr.Message}",
        				Indentation = 6
        			});
        			exPtr = exPtr.InnerException;
        		} while (exPtr!=null);
        		Imported?.Invoke(this,new ImportedEventArgs()
        		{
        			LanguageCode = languageCode,
        			CurrentItem = communityContentItem.Title,
        			IsSuccess = false
        		});
        	}
        		
        	TimeSpan delta = DateTime.Now - startTimeStamp;
        	
        	UpdateMetrics?.Invoke(this,new ImportMetricsEventArgs()
        	{
        		MillisecondsBetweenImports = delta.TotalMilliseconds,
        	});
        	
        }
		        
    }
    

    private async Task<CreateCommunityContentItemResult> PostCommunityContentItemToTarget(HttpClient httpClient, CommunityContentItem communityContentItem,
	    int dictionaryId)
    {
	    string url = "/api/v4/communitycontentitems";
	    CreateCommunityContentItem createCommunityContentItem = new CreateCommunityContentItem()
	    {
		    Title = communityContentItem.Title,
		    AuthorName = communityContentItem.AuthorName,
		    AuthorUrl = communityContentItem.AuthorUrl,
		    CollectionName = communityContentItem.Name,
		    CreatedOn = Environment.MachineName,
		    ImageUrl = communityContentItem.ImageUrl,
		    SourceUrl = communityContentItem.SourceUrl,
		    SynopsisText = communityContentItem.SynopsisText,
		    OriginalSynopsisHtml = communityContentItem.OriginalSynopsisHtml,
		    CreatedByUserName = $"{Environment.UserName}@{Environment.MachineName}",
		    CreatedAt = DateTime.Now,
		    DictionaryId = dictionaryId,
		    ExternalId = $"Taggloo2-CommunityContentItem-{communityContentItem.ID}",
		    IsTruncated = communityContentItem.IsTruncated,
		    PublishedAt = communityContentItem.PublishedTimeStamp,
		    RetrievedAt = communityContentItem.RetrievedTimeStamp
	    };
		
	    HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, createCommunityContentItem);
	    if (!response.IsSuccessStatusCode)
	    {
		    throw new InvalidOperationException($"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
	    }

	    CreateCommunityContentItemResult? createCommunityContentItemResult=
		    await response.Content.ReadFromJsonAsync<CreateCommunityContentItemResult>();
	    return createCommunityContentItemResult!;
    }

    
}