using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Word Translation to be updated using the PATCH Translations method.
/// </summary>
public class UpdateWordTranslation
{
	/// <summary>
	/// Identifier of Dictionary currently containing the Word Translation
	/// </summary>
	public int? DictionaryId { get; set; }


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
	/// If specified, the ID of the originating Word
	/// </summary>
	public int? FromWordId { get; set; }
	
	/// <summary>
	/// If specified, the ID of the target Word
	/// </summary>
	public int? ToWordId { get; set; }
	
}