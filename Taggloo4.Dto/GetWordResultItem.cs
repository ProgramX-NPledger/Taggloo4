namespace Taggloo4.Dto;

/// <summary>
/// Represents a result within <seealso cref="GetWordsResult"/>.
/// </summary>
public class GetWordResultItem
{
	/// <summary>
	/// Identifier of Word.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// The Word.
	/// </summary>
	public required string Word { get; set; }
	
	/// <summary>
	/// Links associated with the Word.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
}