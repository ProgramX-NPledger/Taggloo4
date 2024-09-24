namespace Taggloo4.Model;

public class WordsInDictionariesSummary
{
    public int? DictionaryId { get; set; }
    public string? DictionaryName { get; set; }
    public int? WordCount { get; set; }
    public DateTime? LatestWordCreatedAt { get; set; }
}