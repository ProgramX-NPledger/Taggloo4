using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Phrase to be created using the POST Phrases method.
/// </summary>
public class CreatePhrase
{
	/// <summary>
	/// Identifier of the Dictionary that will contain the Phrase
	/// </summary>
	public int DictionaryId { get; set; }
	
	
	/// <summary>
	/// The Phrase to create, in the language specified by <c>IetfLanguageTag</c>.
	/// </summary>
	[Required]
	public required string Phrase { get; set; }
	
	/// <summary>
	/// If specified, updates the CreatedByUserName field.
	/// </summary>
	public string? CreatedByUserName { get; set; }

	/// <summary>
	/// If specified, updates the CreatedOn field.
	/// </summary>
	public string? CreatedOn { get; set; }
	
	/// <summary>
	/// If specified, updates the CreatedAt field.
	/// </summary>
	public DateTime? CreatedAt { get; set; }
	
	/// <summary>
	/// Assign an external identifier to the entity.
	/// </summary>
	/// <remarks>This should be externally consistent and unique. Taggloo cannot guarantee or assume uniqueness.</remarks>
	public string? ExternalId { get; set; }
	
	public required string IetfLanguageTag { get; set; }
}