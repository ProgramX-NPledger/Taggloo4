﻿using System.ComponentModel.DataAnnotations;

namespace API.Model;

/// <summary>
/// A supported Language.
/// </summary>
public class Language
{
	/// <summary>
	/// Identifier of Language using iETF Language Tag convention. (eg. en-GB)
	/// </summary>
	[Required]
	[Key]
	[MaxLength(16)]
	public string IetfLanguageTag { get; set; }
	
	/// <summary>
	/// Name of Language.
	/// </summary>
	[Required] 
	public string Name { get; set; }

	/// <summary>
	/// <seealso cref="Dictionary"/> entities within the Language.
	/// </summary>
	public ICollection<Dictionary> Dictionaries { get; set; }
	
}