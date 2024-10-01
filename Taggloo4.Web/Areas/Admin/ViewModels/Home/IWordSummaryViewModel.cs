namespace Taggloo4.Web.Areas.Admin.ViewModels.Home;

/// <summary>
/// View-Model for summarising Words for the Admin Dashboard.
/// </summary>
public interface IWordSummaryViewModel
{
    /// <summary>
    /// Total number of available Words, across Dictionaries and Languages.
    /// </summary>
    int TotalWords { get; set; }
    
    /// <summary>
    /// Number of Dictionaries that have Words.
    /// </summary>
    int AcrossDictionariesCount { get; set; }
    
    /// <summary>
    /// Timestamp of last created Word.
    /// </summary>
    DateTime? LastWordCreatedTimeStamp { get; set; }
}