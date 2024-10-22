using Taggloo4.Web.Areas.Admin.Models.DTOs;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Home;

/// <summary>
/// Defines the View Model for the Admin/Home/Index view, including dependent View Models for partial Views.
/// </summary>
public class IndexViewModel : ILanguageSummaryViewModel, IDictionariesSummaryViewModel, IUserSummaryViewModel, IHangfireSummaryViewModel, IWordSummaryViewModel
{
    /// <inheritdoc cref="ILanguageSummaryViewModel"/>
    public IEnumerable<LanguageDto>? AllLanguages { get; set; }

    /// <inheritdoc cref="IHangfireSummaryViewModel"/>
    public int NumberOfRecurringJobs { get; set; }
    /// <inheritdoc cref="IHangfireSummaryViewModel"/>
    public DateTime? LastJobExecution { get; set; }
    /// <inheritdoc cref="IHangfireSummaryViewModel"/>
    public DateTime? NextJobExecution { get; set; }

    /// <inheritdoc cref="IWordSummaryViewModel"/>
    public int TotalWords { get; set; }
    /// <inheritdoc cref="IWordSummaryViewModel"/>
    public int AcrossDictionariesCount { get; set;  }
    /// <inheritdoc cref="IWordSummaryViewModel"/>
    public DateTime? LastWordCreatedTimeStamp { get; set; }
    
    /// <inheritdoc cref="IDictionariesSummaryViewModel.NumberOfLanguagesInDictionaries"/>
    public int NumberOfLanguagesInDictionaries { get; set; }

    /// <inheritdoc cref="IDictionariesSummaryViewModel.NumberOfDictionaries"/>
    public int NumberOfDictionaries { get; set; }

    /// <inheritdoc cref="IDictionariesSummaryViewModel.NumberOfContentTypes"/>
    public int NumberOfContentTypes { get; set; }

    /// <summary>
    /// Whether the User has the Administrator role.
    /// </summary>
    public bool IsAdministrator { get; set; }
    
}