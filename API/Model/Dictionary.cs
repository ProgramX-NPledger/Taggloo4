﻿using System.ComponentModel.DataAnnotations;

namespace API.Model;

/// <summary>
/// A Dictionary has a Language and content such as Words, Phrases, Translations, etc.
/// </summary>
public class Dictionary
{
	/// <summary>
	/// Identifier of Dictionary.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// Name of Dictionary.
	/// </summary>
	[Required] 
	public required string Name { get; set; }

	/// <summary>
	/// Description of Dictionary
	/// </summary>
	[MaxLength(1024)]
	public required string Description { get; set; }

	/// <summary>
	/// URL of source of Dictionary.
	/// </summary>
	public required string SourceUrl { get; set; }

	/// <summary>
	/// IETF Language-tag of the Dictionary. This must be a valid Language.
	/// </summary>
	public required string IetfLanguageTag { get; set; }
	
	/// <summary>
	/// UserName of creator.
	/// </summary>
	public required string? CreatedByUserName { get; set; }

	/// <summary>
	/// Timestamp of creation.
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// Host from which the Dictionary was created.
	/// </summary>
	public required string CreatedOn { get; set; }

	/// <summary>
	/// <seealso cref="Language"/> of Dictionary.
	/// </summary>
	public Language? Language { get; set; }

	/// <summary>
	/// <seealso cref="Word"/>s in Dictionary.
	/// </summary>
	public ICollection<Word>? Words { get; set; }

	/// <summary>
	/// <seealso cref="WordTranslation"/>s in Dictionary.
	/// </summary>
	public ICollection<WordTranslation>? WordTranslations { get; set; }
	
	
}