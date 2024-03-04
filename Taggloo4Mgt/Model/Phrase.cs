namespace Taggloo4Mgt.Model;

public class Phrase
{
	public int ID { get; set; }
	public string ThePhrase { get; set; }
	public required string LanguageCode { get; set; }
	public DateTime CreatedTimeStamp { get; set; }
	public required string CreatedByUserName { get; set; }
	public int DictionaryID { get; set; }
}