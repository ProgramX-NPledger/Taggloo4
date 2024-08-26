namespace API.Translation;

/// <summary>
/// Wraps <seealso cref="TranslationResults"/> with data about the request being made.
/// </summary>
public class TranslationResultsWithMetaData
{
    /// <summary>
    /// The results of the Translation.
    /// </summary>
    public TranslationResults TranslationResults { get; }
    
    /// <summary>
    /// The name of the Translator.
    /// </summary>
    public required string Translator { get; set; }
    
    /// <summary>
    /// The time taken to perform the Translation.
    /// </summary>
    public TimeSpan TimeTaken { get; set; }

    /// <summary>
    /// The original Translation Request by the client.
    /// </summary>
    public TranslationRequest TranslationRequest { get; }
    
    /// <summary>
    /// Unique identifier for the Translation assigned by the job manager.
    /// </summary>
    public string? JobId { get; set;  }
    
    /// <summary>
    /// Instantiates an instance of the object with the pre-configured parameters.
    /// </summary>
    /// <param name="translationResults">The results of the Translation.</param>
    /// <param name="translationRequest">The original Translation Request by the client.</param>
    public TranslationResultsWithMetaData(TranslationResults translationResults, TranslationRequest translationRequest)
    {
        TranslationResults = translationResults;
        TranslationRequest = translationRequest;
    }
    

    /// <summary>
    /// When <c>True</c> is being rendered as a Details View, allowing the View to customise its output between the summary and detail view modes.
    /// </summary>
    public bool IsRenderedAsDetailsView { get; set; } = false;
    
    public int CurrentPageNumber { get; set; } = 1;

    public int? NumberOfPages { get; set; }

    public int? NumberOfItemsPerPage { get; set; }
    
}