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
    private readonly ILanguageRepository _languageRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;

    /// <summary>
    /// Default constructor with injected properties.
    /// </summary>
    /// <param name="languageRepository">Implementation of <seealso cref="ILanguageRepository"/>.</param>
    public TranslateController(ILanguageRepository languageRepository,
        IBackgroundJobClient backgroundJobClient,
        IHubContext<TranslateHub> hubContext)
    {
        _languageRepository = languageRepository;
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
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
        TranslationRequest translationRequest = CreateTranslationRequestFromITranslationRequestViewModel(viewModel);
        Translator translator = new Translator(_backgroundJobClient, _hubContext);
        string id = translator.Translate(translationRequest);

        return View("Translate", new TranslateViewModel()
        {
            TranslationId = id
        });
    }


    private TranslationRequest CreateTranslationRequestFromITranslationRequestViewModel(ITranslationRequestViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Query)) throw new InvalidOperationException("Query cannot be null");
        
        TranslationRequest translationRequest = new TranslationRequest()
        {
            Query = viewModel.Query
        };
        return translationRequest;
    }
}