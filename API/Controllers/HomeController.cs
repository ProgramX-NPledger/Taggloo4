using API.Contract;
using API.Model;
using API.Model.Home;
using API.ViewModels.Home.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Default controller for core routes and messages.
/// </summary>
[Authorize(AuthenticationSchemes = "Identity.Application")]
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
    /// Site home page.
    /// </summary>
    /// <returns>View <c>Index</c>.</returns>
    public async Task<IActionResult> Index()
    {
        IEnumerable<Language> allLanguages = await _languageRepository.GetAllLanguagesAsync();
        
        IndexViewModelFactory viewModelFactory = new IndexViewModelFactory(allLanguages);
        IndexViewModel viewModel = viewModelFactory.Create();
        return View(viewModel);
    }

    /// <summary>
    /// Friendly error message for 401/unauthorised access.
    /// </summary>
    /// <returns>View <c>NotPermitted</c>.</returns>
    public IActionResult NotPermitted()
    {
        return View();
    }
}