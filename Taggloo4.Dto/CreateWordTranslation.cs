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
	
	
}