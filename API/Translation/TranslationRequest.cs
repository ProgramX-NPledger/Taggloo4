﻿namespace API.Translation;

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
}