namespace API.Areas.Admin.ViewModels.Home;

public interface IHangfireSummaryViewModel
{
    int NumberOfRecurringJobs { get; set; }
    DateTime? LastJobExecution { get; set; }
    DateTime? NextJobExecution { get; set; }    
}