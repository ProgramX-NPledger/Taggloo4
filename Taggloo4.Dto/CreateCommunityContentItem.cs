using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Community Content Item to be created using the POST Community Content Items method.
/// </summary>
public class CreateCommunityContentItem
{
	/// <summary>
	/// Title of item.
	/// </summary>
	public required string Title { get; set; }
	
	/// <summary>
	/// Original synopsis of item (without HTML).
	/// </summary>
	public required string SynopsisText { get; set; }
	
	/// <summary>
	/// URL of adjacent image.
	/// </summary>
	public required string ImageUrl { get; set; }
	
	/// <summary>
	/// Name of author.
	/// </summary>
	public required string AuthorName { get; set; }
	
	/// <summary>
	/// URL to author's profile or feed.
	/// </summary>
	public required string AuthorUrl { get; set; }
	
	/// <summary>
	/// URL of original item.
	/// </summary>
	public required string SourceUrl { get; set; }
	
	/// <summary>
	/// Date/time the item was published.
	/// </summary>
	public DateTime PublishedAt { get; set; }

	
	/// <summary>
	/// Short synopsis of item, using original HTML.
	/// </summary>
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
	/// Identifier of the Dictionary that will contain the Phrase
	/// </summary>
	public int DictionaryId { get; set; }

	/// <summary>
	/// UserName of creator.
	/// </summary>
	public required string CreatedByUserName { get; set; }

	/// <summary>
	/// Host/IP Address on which item was created.
	/// </summary>
	public required string CreatedOn { get; set; }
	
	/// <summary>
	/// Timestamp of item's creation.
	/// </summary>
	public DateTime CreatedAt { get; set; }
	
	/// <summary>
	/// Assign an external identifier to the entity.
	/// </summary>
	/// <remarks>This should be externally consistent and unique. Taggloo cannot guarantee or assume uniqueness.</remarks>
	public string? ExternalId { get; set; }

	/// <summary>
	/// Name of collection item should be added to.
	/// </summary>
	public required string CollectionName { get; set; }
	
}