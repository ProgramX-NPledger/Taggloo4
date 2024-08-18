namespace API.Translation;

public class TranslationResultsWithMetaData
{
    public TranslationResults TranslationResults { get; set; }
    public required string Translator { get; set; }
    public TimeSpan TimeTaken { get; set; }

    public TranslationRequest TranslationRequest { get; set; }

    private TranslationResultsWithMetaData()
    {
    }
    
    public TranslationResultsWithMetaData(TranslationResults translationResults, TranslationRequest translationRequest)
    {
        TranslationResults = translationResults;
        TranslationRequest = translationRequest;
    }
}