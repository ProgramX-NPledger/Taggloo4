using System.Runtime.Serialization;

namespace Taggloo4.Model.Exceptions;

public class ImportException : Exception
{
    public string? Text { get; set; }
    public string? Type { get; set; }
    
    /// <summary>
    /// Instantiates a default instance.
    /// </summary>
    public ImportException() : base("An error occurred during import")
    {
    }
    
    /// <summary>
    /// Instantiates a new instance with the configured message.
    /// </summary>
    /// <param name="message">Message to include in the Exception.</param>
    public ImportException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Instantiates a new instance with the configured message and inner Exception.
    /// </summary>
    /// <param name="message">Message to include in the Exception.</param>
    /// <param name="innerException">Inner Exception to include in the Exception.</param>
    public ImportException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Instantiates a new instance with the configured inner exception with a default message.
    /// </summary>
    /// <param name="innerException">Inner Exception to include in the Exception.</param>
    public ImportException(Exception? innerException) : base("An error occurred during import", innerException)
    {
    }

    /// <summary>
    /// Instantiates a new instance with for the configured <c>text</c> imported and <c>type</c>.
    /// Creates a default message.
    /// </summary>
    /// <param name="text">Data being imported.</param>
    /// <param name="type">Type of import.</param>
    public ImportException(string text, string type) : base($"An error occurred importing {type} '{text}'")
    {
    }

    /// <summary>
    /// Instantiates a new instance with for the configured <c>text</c> imported and <c>type</c>.
    /// Creates a default message.
    /// </summary>
    /// <param name="text">Data being imported.</param>
    /// <param name="type">Type of import.</param>
    /// <param name="innerException">Inner Exception to include in the Exception.</param>
    public ImportException(string text, string type, Exception? innerException) : base(
        $"An error occurred importing {type} '{text}'", innerException)
    {
    }
    
}