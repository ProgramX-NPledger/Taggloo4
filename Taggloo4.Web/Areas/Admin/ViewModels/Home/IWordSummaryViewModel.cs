namespace Taggloo4.Web.Areas.Admin.ViewModels.Home;

public interface IWordSummaryViewModel
{
    int TotalWords { get; set; }
    int AcrossDictionariesCount { get; set; }
    DateTime? LastWordCreatedTimeStamp { get; set; }
}