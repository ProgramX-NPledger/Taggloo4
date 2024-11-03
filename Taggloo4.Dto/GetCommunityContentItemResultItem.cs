using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a result within <seealso cref="GetPhrasesResult"/>.
/// </summary>
public class GetCommunityContentItemResultItem
{
	/// <summary>
    /// Identifier of Community Content Item.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Title
    /// </summary>
    [MaxLength(256)]
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    
    /// <summary>
    /// Short synopsis of item.
    /// </summary>
    [MaxLength(512)]
    [JsonPropertyName("synopsisText")]
    public required string SynopsisText { get; set; }
    
    /// <summary>
    /// URL to adjacent image.
    /// </summary>
    [MaxLength(256)]
    [JsonPropertyName("imageUrl")]
    public required string ImageUrl { get; set; }
    
    /// <summary>
    /// Name of author.
    /// </summary>
    [MaxLength(64)]
    [JsonPropertyName("authorName")]
    public required string AuthorName { get; set; }
    
    /// <summary>
    /// URL to author's biography or feed.
    /// </summary>
    [MaxLength(256)]
    [JsonPropertyName("authorUrl")]
    public required string AuthorUrl { get; set; }
    
    /// <summary>
    /// Hash identifying the content, allowing rapid comparison.
    /// </summary>
    [MaxLength(64)]
    [JsonPropertyName("hash")]
    public required string Hash { get; set; }
    
    /// <summary>
    /// Date/time the item was published.
    /// </summary>
    [JsonPropertyName("publishedAt")]
    public DateTime PublishedAt { get; set; }
    
    /// <summary>
    /// Algorithm used to generate the hash.
    /// </summary>
    [MaxLength(16)]
    [JsonPropertyName("hashAlgorithm")]
    public required string HashAlgorithm { get; set; }

    /// <summary>
    /// Short synopsis of item, using original HTML.
    /// </summary>
    [MaxLength(1024)]
    [JsonPropertyName("originalSynopsisHtml")]
    public required string OriginalSynopsisHtml { get; set; }

    /// <summary>
    /// Set if the content has been truncated.
    /// </summary>
    [JsonPropertyName("isTrunacted")]
    public bool IsTruncated { get; set; }

    /// <summary>
    /// Date/time the item was captured and recorded within Taggloo.
    /// </summary>
    [JsonPropertyName("retrievedAt")]
    public DateTime RetrievedAt { get; set; }

    /// <summary>
    /// Identifier of Dictionary.
    /// </summary>
    [JsonPropertyName("dictionaryId")]
    public int DictionaryId { get; set; }

    /// <summary>
    /// Identifier of owning collection.
    /// </summary>
    [JsonPropertyName("communityContentCollectionId")]
    public int CommunityContentCollectionId { get; set; }

    /// <summary>
    /// Links associated with the item.
    /// </summary>
    [JsonPropertyName("links")]
    public IEnumerable<Link>? Links { get; set; }

    /// <summary>
    /// IETF Language Tag of Dictionary.
    /// </summary>
    public required string IetfLanguageTag { get; set; }
	
}