using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Phrase Translation to be created using the POST Translations method.
/// </summary>
public class CreatePhraseTranslation
{
	/// <summary>
	/// ID of Phrase to translate.
	/// </summary>
	[Required]
	public int FromPhraseId { get; set; }

	/// <summary>
	/// The translated Phrase
	/// </summary>
	[Required]
	public int ToPhraseId { get; set; }
	
	
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