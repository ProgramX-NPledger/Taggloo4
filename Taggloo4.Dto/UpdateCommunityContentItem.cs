using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Phrase to be updated using the PATCH Phrases method.
/// </summary>
public class UpdateCommunityContentItem
{


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