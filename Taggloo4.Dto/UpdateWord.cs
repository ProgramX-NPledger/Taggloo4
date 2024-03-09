using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Word to be updated using the PATCH Words method.
/// </summary>
public class UpdateWord
{
	/// <summary>
	/// Identifier of Dictionary currently containing the Word
	/// </summary>
	public int DictionaryId { get; set; }
	

	/// <summary>
	/// If specified, causes the Word to ve moved to another Dictionary. Identifier of the Dictionary that will contain the Word
	/// </summary>
	public int? MoveWordToDictionaryId { get; set; }

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
	/// The ID of the Word to update.
	/// </summary>
	public int WordId { get; set; }

	
	
}