using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Model;

/// <summary>
/// An item that has been retrieved from the internet.
/// </summary>
public class CommunityContentItem
{
    /// <summary>
    /// Identifier of Community Content Item.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Title
    /// </summary>
    [MaxLength(256)]
    public required string Title { get; set; }
    
    /// <summary>
    /// Short synopsis of item.
    /// </summary>
    [MaxLength(512)]
    public required string SynopsisText { get; set; }
    
    /// <summary>
    /// URL to adjacent image.
    /// </summary>
    [MaxLength(256)]
    public required string ImageUrl { get; set; }
    
    /// <summary>
    /// Name of author.
    /// </summary>
    [MaxLength(64)]
    public required string AuthorName { get; set; }
    
    /// <summary>
    /// URL to author's biography or feed.
    /// </summary>
    [MaxLength(256)]
    public required string AuthorUrl { get; set; }

    /// <summary>
    /// URL of original item.
    /// </summary>
	[MaxLength(256)]
    public required string SourceUrl { get; set; }
    
    /// <summary>
    /// Hash identifying the content, allowing rapid comparison.
    /// </summary>
    [MaxLength(64)]
    public required string Hash { get; set; }
    
    /// <summary>
    /// Date/time the item was published.
    /// </summary>
    public DateTime PublishedAt { get; set; }
    
    /// <summary>
    /// Algorithm used to generate the hash.
    /// </summary>
    [MaxLength(16)]
    public required string HashAlgorithm { get; set; }

    /// <summary>
    /// Short synopsis of item, using original HTML.
    /// </summary>
    [MaxLength(1024)]
    public required string OriginalSynopsisHtml { get; set; }

    /// <summary>
    /// Set if the content has been truncated as it was retrieved.
    /// </summary>
    public bool IsTruncated { get; set; }

    /// <summary>
    /// Date/time the item was captured and recorded within Taggloo.
    /// </summary>
    public DateTime RetrievedAt { get; set; }

    /// <summary>
    /// Identifier of Dictionary.
    /// </summary>
    public int DictionaryId { get; set; }

    /// <summary>
    /// Navigational property to Dictionary.
    /// </summary>
    public Dictionary? Dictionary { get; set; }

    /// <summary>
    /// Identifier of owning collection.
    /// </summary>
    public int CommunityContentCollectionId { get; set; }

    /// <summary>
    /// Navigational property to owning collection.
    /// </summary>
    public CommunityContentCollection? CommunityContentCollection { get; set; }
    
    /// <summary>
    /// UserName of creator.
    /// </summary>
    public string? CreatedByUserName { get; set; }

    /// <summary>
    /// Host name/address of creator.
    /// </summary>
    public string? CreatedOn { get; set; }
	
    /// <summary>
    /// Timestamp of creation.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
	
    /// <summary>
    /// Assign an external identifier to the entity.
    /// </summary>
    /// <remarks>This should be externally consistent and unique. Taggloo cannot guarantee or assume uniqueness.</remarks>
    public string? ExternalId { get; set; }
    
}