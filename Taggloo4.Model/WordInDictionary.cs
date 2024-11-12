namespace Taggloo4.Model;

public class WordInDictionary
{
    public int? WordId { get; set; }
    public string? TheWord { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedOn { get; set; }
    public string? ExternalId { get; set; }
    public int? DictionaryId { get; set; }
    public string? DictionaryName { get; set; }
    public string? ContentTypeFriendlyName { get; set; }
    public string? IetfLanguageTag { get; set; }
    public string? LanguageName { get; set; }

    public int? AppearsInPhrasesCount { get; set; }
    
    
}