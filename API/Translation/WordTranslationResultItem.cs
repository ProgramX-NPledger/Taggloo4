namespace API.Translation;

public class WordTranslationResultItem : TranslationResultItem
{
    public int FromWordId { get; set; }
    public int ToWordId { get; set; }
}