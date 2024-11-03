namespace Taggloo4Mgt.Importing.Model;

public class CommunityContentItem
{
    public int ID { get; set; }
    public required string Title { get; set; }
    public required string SynopsisText { get; set; }
    public required string ImageUrl { get; set; }
    public required string AuthorName { get; set; }
    public required string AuthorUrl { get; set; }
    public required string SourceUrl { get; set; }
    public DateTime PublishedTimeStamp { get; set; }
    public required string Hash { get; set; }
    public required string OriginalSynopsisHtml { get; set; }
    public bool IsTruncated { get; set; }
    public DateTime RetrievedTimeStamp { get; set; }
    public int DiscovererID { get; set; }
    public required string DotNetTypeName { get; set; }
    public bool IsEnabled { get; set; }
    public required string Name { get; set; }
    public required string LanguageCode { get; set; }
    
    
}