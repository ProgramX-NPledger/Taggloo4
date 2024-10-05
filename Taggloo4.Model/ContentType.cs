using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Model;

/// <summary>
/// A Content Type is the type of content wihtin a Dictionary (eg. Word, Phrase, etc.)
/// </summary>
public class ContentType
{
    /// <summary>
    /// Identifier of Content Type.
    /// </summary>
    public int Id { get; set; }
	
    /// <summary>
    /// Disambiguated identifier for type of content to allow automatic processing.
    /// </summary>
    [MaxLength(32)]
    public required string ContentTypeKey { get; set; }
    
    /// <summary>
    /// Name of Content Type
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The URL name of the Controller to use to retrieve content.
    /// </summary>
    [MaxLength(32)]
    public required string Controller { get; set; }

    /// <summary>
    /// <seealso cref="Dictionary"/> items using this Content Type.
    /// </summary>
    public ICollection<Dictionary> Dictionaries { get; set; }
}