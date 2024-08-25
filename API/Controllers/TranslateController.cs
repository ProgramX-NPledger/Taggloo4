using System.Reflection;
using API.Contract;
using API.Data;
using API.Hubs;
using API.Model;
using API.Model.Home;
using API.RazorViewRendering;
using API.Translation;
using API.Translation.Utility;
using API.ViewModels;
using API.ViewModels.Home.Factory;
using API.ViewModels.Translate;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API.Controllers;

/// <summary>
/// Translation controller.
/// </summary>
[Authorize(AuthenticationSchemes = "Identity.Application")]
[AllowAnonymous]

public class TranslateController : Controller
{
    private readonly DataContext _dataContext;
    private readonly ITranslatorRepository _translatorRepository;

    /// <summary>
    /// Default constructor with injected properties.
    /// </summary>
    public TranslateController(DataContext dataContext, ITranslatorRepository translatorRepository)
    {
        _dataContext = dataContext;
        _translatorRepository = translatorRepository;
    }
    
    /// <summary>
    /// Site home page.
    /// </summary>
    /// <returns>View <c>Index</c>.</returns>
    public async Task<IActionResult> Index()
    {
        // return full translation search form
        return View();
    }

    /// <summary>
    /// Translation results page.
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    /// <remarks>Translation results are returned asynchronously having been submitted using the Hangfire job management API and
    /// are returned by SignaLR.</remarks>
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Translate(TranslateViewModel viewModel)
    {
        return View("Translate", new TranslateViewModel()
        {
     
        });
    }

    public async Task<IActionResult> Details(TranslateViewModel viewModel)
    {
        DateTime startTimeStamp = DateTime.Now;
        AssertValidTranslators(viewModel.Translators);

        ITranslatorFactory translatorFactory = await GetTranslatorFactoryAsync(viewModel.Translators.Single());
        ITranslator translator=translatorFactory.Create(_dataContext);

        TranslationRequest translationRequest = TranslationRequestUtility.CreateTranslationRequestFromTranslateViewModel(viewModel,null);
        TranslationResults translationResults = translator.Translate(translationRequest);
        if (translationResults.ResultItems == null)
            throw new NotImplementedException("Translator reported a null result");
        
        // translation results for summaries are truncated and the paginator is not displayed
        int maximumNumberOfItemsToDisplay = 10; // TODO this should be a configuration
        translationResults.MaximumItems = maximumNumberOfItemsToDisplay < translationResults.ResultItems.Count() ? maximumNumberOfItemsToDisplay : translationResults.ResultItems.Count(); 
        translationResults.ShowPaginator = true;
        
        TimeSpan delta = DateTime.Now - startTimeStamp;
        TranslationResultsWithMetaData translationResultsWithMetaData =
            new TranslationResultsWithMetaData(translationResults,translationRequest)
            {
                Translator = translator.GetType().Name,
                TimeTaken = delta,
                JobId = null
            }; 
        
        string viewName = $"Views\\Translate\\{translator.GetType().Name}.cshtml";
        return View(viewName, translationResultsWithMetaData);
    }

    private async Task<ITranslatorFactory> GetTranslatorFactoryAsync(string translatorKey)
    {
        Model.Translator? translator = (await _translatorRepository.GetAllTranslatorsAsync(translatorKey, null, null, true)).FirstOrDefault();
        if (translator == null)
            throw new InvalidOperationException($"Could not resolve {nameof(ITranslatorFactory)} '{translatorKey}'");
        
        // get the assembly
        Assembly assembly = Assembly.Load(translator.DotNetFactoryAssembly);
        
        // get the type
        Type? type = assembly.GetType(translator.DotNetFactoryType);
        if (type == null)
            throw new InvalidOperationException(
                $"Failed to locate {nameof(ITranslatorFactory)} '{translator.DotNetFactoryType}'.");
        
        // instantiate the type
        object? o = Activator.CreateInstance(type);
        if (o == null)
            throw new InvalidOperationException($"Failed to activate '{type.Name}");
        
        // return the type
        ITranslatorFactory? iTranslatorFactory=o as ITranslatorFactory;
        if (iTranslatorFactory == null)
            throw new InvalidOperationException(
                $"Implementation of {o.GetType().Name} does not implement {nameof(ITranslatorFactory)}");

        return iTranslatorFactory;
    }


    private void AssertValidTranslators(IEnumerable<string> translators)
    {
        if (translators == null) throw new ArgumentNullException(nameof(translators));
        if (!translators.Any()) throw new InvalidOperationException("Translators cannot be empty");
        if (translators.Count() > 1) throw new InvalidOperationException("Only a single Translator is supported");
    }
}