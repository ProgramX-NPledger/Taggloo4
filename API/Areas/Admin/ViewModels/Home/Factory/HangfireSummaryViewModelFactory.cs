using API.Areas.Admin.Models.DTOs;
using API.Contract;
using API.Model;
using API.ViewModels.Home;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.Areas.Admin.ViewModels.Home.Factory;

/// <summary>
/// Configures non-transient data for a View Model implementation of <seealso cref="IHangfireSummaryViewModel"/>.
/// </summary>
public class HangfireSummaryViewModelFactory : IPartialViewModelFactory<IHangfireSummaryViewModel>
{
    private readonly int _numberOfRecurringHangfireJobs;
    private readonly DateTime? _lastHangfireJobExecution;
    private readonly DateTime? _nextHangfireJobExecution;
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    public HangfireSummaryViewModelFactory(int numberOfRecurringHangfireJobs, DateTime? lastHangfireJobExecution, DateTime? nextHangfireJobExecution)
    {
        _numberOfRecurringHangfireJobs = numberOfRecurringHangfireJobs;
        _lastHangfireJobExecution = lastHangfireJobExecution;
        _nextHangfireJobExecution = nextHangfireJobExecution;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref IHangfireSummaryViewModel viewModel)
    {
        viewModel.LastJobExecution=_lastHangfireJobExecution;
        viewModel.NextJobExecution = _nextHangfireJobExecution;
        viewModel.NumberOfRecurringJobs = _numberOfRecurringHangfireJobs;
    }

}