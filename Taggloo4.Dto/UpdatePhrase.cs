using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Phrase to be updated using the PATCH Phrases method.
/// </summary>
public class UpdatePhrase
{
	/// <summary>
	/// Identifier of Dictionary currently containing the Phrase
	/// </summary>
	public int DictionaryId { get; set; }
	
	
	/// <summary>
	/// The Phrase to update, in the language specified by <c>IetfLanguageTag</c>. Changing this will have the effect of
	/// changing the word and compromising indexes.
	/// </summary>
	public string? Phrase { get; set; }

	/// <summary>
	/// If specified, causes the Phrase to be moved to another Dictionary. Identifier of the Dictionary that will contain the Phrase
	/// </summary>
	public int? MovePhraseToDictionaryId { get; set; }

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
	

	[Required] public int PhraseId { get; set; }
	
}