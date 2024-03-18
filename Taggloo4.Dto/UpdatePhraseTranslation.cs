using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Phrase Translation to be updated using the PATCH Translations method.
/// </summary>
public class UpdatePhraseTranslation
{
	/// <summary>
	/// Identifier of Dictionary currently containing the Phrase Translation
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
	/// If specified, the ID of the originating Phrase
	/// </summary>
	public int? FromPhraseId { get; set; }
	
	/// <summary>
	/// If specified, the ID of the target Phrase
	/// </summary>
	public int? ToPhraseId { get; set; }
	
}