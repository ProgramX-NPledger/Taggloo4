namespace API.Areas.Admin.ViewModels.Home;

/// <summary>
/// Provides summary information about Hangfire.
/// </summary>
public interface IHangfireSummaryViewModel
{
    /// <summary>
    /// Number of recurring Jobs.
    /// </summary>
    int NumberOfRecurringJobs { get; set; }
    
    /// <summary>
    /// Date/time of last Job execution, <c>null</c> if no Jobs yet executed.
    /// </summary>
    DateTime? LastJobExecution { get; set; }
    
    /// <summary>
    /// Date/time of next Job execution, <c>null</c> if no Jobs are scheduled.
    /// </summary>
    DateTime? NextJobExecution { get; set; }    
}