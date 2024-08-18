using API.Contract;
using API.Hubs;
using API.Model;
using API.Model.Home;
using API.Translation;
using API.ViewModels;
using API.ViewModels.Home.Factory;
using API.ViewModels.Translate;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers;

/// <summary>
/// Translation controller.
/// </summary>
[Authorize(AuthenticationSchemes = "Identity.Application")]
[AllowAnonymous]

public class TranslateController : Controller
{

    /// <summary>
    /// Default constructor with injected properties.
    /// </summary>
    public TranslateController()
    {
    }
    
    /// <summary>
    /// Site home page.
    /// </summary>
    /// <returns>View <c>Index</c>.</returns>
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Translate(TranslateViewModel viewModel)
    {
        return View("Translate", new TranslateViewModel()
        {
     
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> RenderResult(string translator)
    {
        // return partial with html suitable for KO binding
        return PartialView($"TranslatorPartialViews/{translator}");
    }


}