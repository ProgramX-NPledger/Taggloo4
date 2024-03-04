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
	
	
}