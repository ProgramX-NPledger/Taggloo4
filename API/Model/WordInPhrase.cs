namespace API.Model;

public class WordInPhrase
{
    public int Id { get; set; }
    public int InPhraseId { get; set; }
    public int WordId { get; set; }
    public int Ordinal { get; set; }
    public Phrase InPhrase { get; set; }
    public Word Word { get; set; }
    public required string CreatedByUserName { get; set; }
    public required string CreatedOn { get; set; }
    public DateTime CreatedAt { get; set; }
    
    
}