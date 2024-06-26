﻿namespace Taggloo4Mgt.Importing.Model;

public class PhraseTranslation
{
	public required string Translation { get; set; }
	public required string LanguageCode { get; set; }
	public DateTime CreatedAt { get; set; }
	public required string CreatedByUserName { get; set; }
	public int FromPhraseId { get; set; }
	public int ToPhraseId { get; set; }
	public int Id { get; set; }
	public required string FromPhrase { get; set; }
}