using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Word Translation to be created using the POST Translations method.
/// </summary>
public class CreateWordTranslation
{
	/// <summary>
	/// ID of Word to translate.
	/// </summary>
	[Required]
	public int FromWordId { get; set; }

	/// <summary>
	/// The translated word
	/// </summary>
	[Required]
	public int ToWordId { get; set; }
	
	
	/// <summary>
	/// Identifier of the Dictionary that will contain the Translation
	/// </summary>
	[Required]
	public int DictionaryId { get; set; }
	
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