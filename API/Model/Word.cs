using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Model;

public class Word
{
	public int Id { get; set; }
	
	[Required] 
	public string TheWord { get; set; }
	
	public required string CreatedByUserName { get; set; }

	public required DateTime CreatedAt { get; set; }

	public required string CreatedOn { get; set; }

	public Dictionary Dictionary { get; set; }
	
	public int DictionaryId { get; set; }

	// this fails when building migrations
//	public ICollection<WordTranslation> Translations { get; set; }

	
	
}