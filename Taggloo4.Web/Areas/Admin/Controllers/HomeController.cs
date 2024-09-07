using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Model;
using Taggloo4.Web.Areas.Admin.ViewModels.Home;
using Taggloo4.Web.Areas.Admin.ViewModels.Home.Factory;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Controllers.Admin;

/// <summary>
/// Controller for Admin area actions.
/// </summary>
/// <remarks>
/// This Controller explicitly sets the desired AuthenticationScheme to avoid conflict with JWT authentication.
/// </remarks>
[Authorize(AuthenticationSchemes = "Identity.Application")]
[Area("Admin")]
[Authorize(Roles = "administrator")]
public class HomeController : Controller
{
    private readonly ILanguageRepository _languageRepository;
    
    /// <summary>
    /// Default constructor with injected properties.
    /// </summary>
    /// <param name="languageRepository">Implementation of <seealso cref="ILanguageRepository"/>.</param>
    public HomeController(ILanguageRepository languageRepository)
    {
        _languageRepository = languageRepository;
    }
    
    /// <summary>
    /// Default page for admin users.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index()
    {
        IEnumerable<Language> allLanguages = await _languageRepository.GetAllLanguagesAsync();

        int numberOfRecurringHangfireJobs;
        DateTime? latestHangireJobExecution;
        DateTime? nextHangfireJobExecution;
        using (var connection = JobStorage.Current.GetConnection())
        {
            var recurringJobs = connection.GetRecurringJobs();
            numberOfRecurringHangfireJobs = recurringJobs.Count;
            latestHangireJobExecution = recurringJobs.MaxBy(q => q.LastExecution)?.LastExecution;
            nextHangfireJobExecution = recurringJobs.MinBy(q => q.NextExecution)?.NextExecution;
            // foreach (var recurringJob in recurringJobs)
            // {
            //     if (NonRemovableJobs.ContainsKey(recurringJob.Id)) continue;
            //     logger.LogWarning($"Removing job with id [{recurringJob.Id}]");
            //     jobManager.RemoveIfExists(recurringJob.Id);
            // }
        }
        
        IndexViewModelFactory viewModelFactory = new IndexViewModelFactory(allLanguages,numberOfRecurringHangfireJobs,latestHangireJobExecution,nextHangfireJobExecution);
        IndexViewModel viewModel = viewModelFactory.Create();
        return View(viewModel);
    }
    
}