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
}