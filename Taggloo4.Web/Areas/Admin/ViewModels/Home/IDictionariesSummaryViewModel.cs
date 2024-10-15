namespace Taggloo4.Web.Areas.Admin.ViewModels.Home;

/// <summary>
/// View-Model for summarising Dictionaries for the Admin Dashboard.
/// </summary>
public interface IDictionariesSummaryViewModel
{
    /// <summary>
    /// Number of Languages across Dictionaries.
    /// </summary>
    /// <remarks>
    /// This should be 2.
    /// </remarks>
    public int NumberOfLanguagesInDictionaries { get; set; }

    /// <summary>
    /// Number of Dictionaries.
    /// </summary>
    public int NumberOfDictionaries { get; set; }

    /// <summary>
    /// Number of Content Types in all known Dictionaries.
    /// </summary>
    public int NumberOfContentTypes { get; set; }
}