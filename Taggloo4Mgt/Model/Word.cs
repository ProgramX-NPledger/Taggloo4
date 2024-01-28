namespace Taggloo4Mgt.Model;

public class Word
{
	public int ID { get; set; }
	public required string TheWord { get; set; }
	public required string LanguageCode { get; set; }
	public DateTime CreatedTimeStamp { get; set; }
	public required string CreatedByUserName { get; set; }
	public bool IsBlocked { get; set; }
	public string? BlockedByUserName { get; set; }
	public DateTime? BlockedTimeStamp { get; set; }
}