namespace API.Translation;

/// <summary>
/// Represents a Translation request of the <seealso cref="AsynchronousTranslatorSession"/> object.
/// </summary>
public class TranslationRequest
{
    /// <summary>
    /// The text to translate.
    /// </summary>
    public required string Query { get; set; }
    
    /// <summary>
    /// A unique identifier for the client, if available, for purposes of communicating back to the original client.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// IETF Language Tag to translate from.
    /// </summary>
    public required string FromLanguageCode { get; set; }

    /// <summary>
    /// IETF Language Tag to translate to.
    /// </summary>
    public required  string ToLanguageCode { get; set; }

    /// <summary>
    /// Ordinal of the first result (useful when paging).
    /// </summary>
    public int OrdinalOfFirstResult { get; set; }

    /// <summary>
    /// Maximum number of results in this page (useful when paging).
    /// </summary>
    public int MaximumNumberOfResults { get; set; }
    
}