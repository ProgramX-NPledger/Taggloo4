namespace Taggloo4.Utility;

/// <summary>
/// Utilities for type conversions.
/// </summary>
public static class TypeConverter
{
    /// <summary>
    /// Attempts to convert a nullable <c>string</c> to a nullable <c>Int32</c>.
    /// </summary>
    /// <param name="s">String to convert.</param>
    /// <param name="throwExceptionOnFailureToParse"></param>
    /// <returns>If <c>s</c> is null or empty, returns <c>null</c>. If <c>s</c> cannot be parsed into
    /// an <c>Int32</c> and <c>throwExceptionOnFailureToParse</c> is <c>true</c>, throws an exception. Returns
    /// <c>null</c> otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown if <c>s</c> cannot be parsed into an <c>Int32</c> and the
    /// <c>throwExceptionOnFailureToParse</c> parameter is <c>true</c>.</exception>
    public static int? ConvertNullableStringToNullableInt(string? s, bool throwExceptionOnFailureToParse = false)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (int.TryParse(s, out var i)) return i;
        if (throwExceptionOnFailureToParse) throw new ArgumentException($"'{s}' is not a valid integer.");
        return null;
    }
}