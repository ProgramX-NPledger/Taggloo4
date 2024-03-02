using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Dictionary to be deleted and options using the DELETE Dictionaries method.
/// </summary>
public class DeleteDictionary
{
	/// <summary>
	/// Identifier of the Dictionary that will be deleted.
	/// </summary>
	public int DictionaryId { get; set; }
}