using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Word to be created using the POST Words method.
/// </summary>
public class CreateWord
{
	/// <summary>
	/// Identifier of the Dictionary that will contain the Word
	/// </summary>
	public int DictionaryId { get; set; }
	
	
	/// <summary>
	/// The Word to create, in the language specified by <c>IetfLanguageTag</c>.
	/// </summary>
	[Required]
	public required string Word { get; set; }

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

	
}