namespace Taggloo4.Dto;

/// <summary>
/// Represents a result within <seealso cref="GetDictionariesResult"/>.
/// </summary>
public class GetDictionaryResultItem
{
	/// <summary>
	/// Identifier of Dictionary.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// Name of Dictionary.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Description of Dictionary
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// URL of source of Dictionary.
	/// </summary>
	public required string SourceUrl { get; set; }

	/// <summary>
	/// IETF Language-tag of the Dictionary. This must be a valid Language.
	/// </summary>
	public required string IetfLanguageTag { get; set; }
	
	/// <summary>
	/// UserName of creator.
	/// </summary>
	public required string? CreatedByUserName { get; set; }

	/// <summary>
	/// Timestamp of creation.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// Host from which the Dictionary was created.
	/// </summary>
	public required string CreatedOn { get; set; }

	/// <summary>
	/// Links associated with the Dictionary.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
	/// <summary>
	/// The URL name of the Controller to use to retrieve content.
	/// </summary>
	public string? Controller { get; set; }

	/// <summary>
	/// Disambiguated identifier for type of content to allow automatic processing.
	/// </summary>
	public string? ContentTypeKey { get; set; }

	/// <summary>
	/// Human-suitable name of Content Type (singular).
	/// </summary>
	public string? NameSingular { get; set; }

	/// <summary>
	/// Human-suitable name of Content Type (plural).
	/// </summary>
	public string? NamePlural { get; set; }

}