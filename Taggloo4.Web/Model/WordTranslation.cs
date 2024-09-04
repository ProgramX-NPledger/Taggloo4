using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Web.Model;

/// <summary>
/// Translation of a <seealso cref="Word"/>.
/// </summary>
public class WordTranslation
{
	/// <summary>
	/// Identifier of the Translation.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// The identifier of the <seealso cref="Word"/> being translated.
	/// </summary>
	public int FromWordId { get; set; }
	
	/// <summary>
	/// Word being translated.
	/// </summary>
	public Word? FromWord { get; set; }
	
	/// <summary>
	/// The identifier of the <seealso cref="Word"/> translation.
	/// </summary>
	public int ToWordId { get; set; }
	
	/// <summary>
	/// The Word being translated to.
	/// </summary>
	public Word? ToWord { get; set; }
	
	/// <summary>
	/// The Host from which the Word Translation was created.
	/// </summary>
	public string? CreatedOn { get; set; }
	
	/// <summary>
	/// The UserName of the creator of the Translation.
	/// </summary>
	public string? CreatedByUserName { get; set; }
	
	/// <summary>
	/// The timestamp of creation of the Translation.
	/// </summary>
	public DateTime CreatedAt { get; set; }
	
	/// <summary>
	/// Identifier of the <seealso cref="Dictionary"/> owner of the Translation.
	/// </summary>
	public int DictionaryId { get; set; }
	
	
	/// <summary>
	/// Owning <seealso cref="Dictionary"/> of the Translation.
	/// </summary>
	public Dictionary? Dictionary { get; set; }
	
	
	
}