using API.Model.Home;
using API.ViewModels.Home.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Default controller for core routes and messages.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Site home page.
    /// </summary>
    /// <returns>View <c>Index</c>.</returns>
    public IActionResult Index()
    {
        IndexViewModelFactory viewModelFactory = new IndexViewModelFactory();
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