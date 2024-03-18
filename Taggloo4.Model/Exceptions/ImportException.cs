using System.Runtime.Serialization;

namespace Taggloo4.Model.Exceptions;

public class ImportException : Exception
{
    public string Text { get; set; }
    public string Type { get; set; }
    
    public ImportException() : base("An error occurred during import")
    {
    }

    protected ImportException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ImportException(string? message) : base(message)
    {
    }

    public ImportException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public ImportException(Exception? innerException) : base("An error occurred during import", innerException)
    {
    }

    public ImportException(string text, string type) : base($"An error occurred importing {type} '{text}'")
    {
    }

    public ImportException(string text, string type, Exception? innerException) : base(
        $"An error occurred importing {type} '{text}'", innerException)
    {
    }
    
}