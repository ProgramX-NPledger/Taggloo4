using Taggloo4.Web.Areas.Admin.Models.DTOs;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Home;

/// <summary>
/// Defines the View Model for the Admin/Home/Index view, including dependent View Models for partial Views.
/// </summary>
public class IndexViewModel : ILanguageSummaryViewModel, IDictionarySummaryViewModel, IUserSummaryViewModel, IHangfireSummaryViewModel, IWordSummaryViewModel
{
    /// <inheritdoc cref="ILanguageSummaryViewModel"/>
    public IEnumerable<LanguageDto>? AllLanguages { get; set; }

    /// <inheritdoc cref="IHangfireSummaryViewModel"/>
    public int NumberOfRecurringJobs { get; set; }
    /// <inheritdoc cref="IHangfireSummaryViewModel"/>
    public DateTime? LastJobExecution { get; set; }
    /// <inheritdoc cref="IHangfireSummaryViewModel"/>
    public DateTime? NextJobExecution { get; set; }

    public int TotalWords { get; set; }
    public int AcrossDictionariesCount { get; set;  }
    public DateTime? LastWordCreatedTimeStamp { get; set; }
}