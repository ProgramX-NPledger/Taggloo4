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
}