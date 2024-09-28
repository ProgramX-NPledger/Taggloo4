using System.Reflection;
using Taggloo4.Web.Hubs;
using Taggloo4.Web.Model;
using Taggloo4.Web.Model.Home;
using Taggloo4.Web.ViewModels;
using Taggloo4.Web.ViewModels.Home.Factory;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Taggloo4.Contract;
using Taggloo4.Contract.Translation;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Translation;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Data;
using Taggloo4.Web.RazorViewRendering;
using Taggloo4.Web.Translation;
using Taggloo4.Web.Translation.Utility;
using Taggloo4.Web.ViewModels.Translate;

namespace Taggloo4.Web.Controllers;

/// <summary>
/// Translation controller.
/// </summary>
[Authorize(AuthenticationSchemes = "Identity.Application")]
[AllowAnonymous]

public class TranslateController : Controller
{
    private readonly DataContext _dataContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ITranslatorConfigurationRepository _translatorRepository;
    private readonly TranslationFactoryService _translationFactoryService;
    private readonly TranslatorConfigurationCache _translatorConfigurationCache;
    private readonly ITranslatorConfigurationRepository _translatorConfigurationRepository;

    /// <summary>
    /// Default constructor with injected properties.
    /// </summary>
    public TranslateController(DataContext dataContext, 
        IWebHostEnvironment webHostEnvironment, 
        ITranslatorConfigurationRepository translatorRepository, 
        TranslationFactoryService translationFactoryService,
        TranslatorConfigurationCache translatorConfigurationCache,
        ITranslatorConfigurationRepository translatorConfigurationRepository)
    {
        _dataContext = dataContext;
        _webHostEnvironment = webHostEnvironment;
        _translatorRepository = translatorRepository;
        _translationFactoryService = translationFactoryService;
        _translatorConfigurationCache = translatorConfigurationCache;
        _translatorConfigurationRepository = translatorConfigurationRepository;
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
    /// <remarks>Translation results are returned asynchronously having been submitted using the Hangfire job management Taggloo4.Web and
    /// are returned by SignaLR.</remarks>
    [HttpGet]
    [HttpPost]
    public IActionResult Translate(TranslateViewModel viewModel)
    {
        return View("Translate", new TranslateViewModel()
        {
     
        });
    }

    /// <summary>
    /// Action for provision of detailed translations for a single Translator.
    /// </summary>
    /// <param name="viewModel">Incoming view model.</param>
    /// <returns>A rendered view containing the results, including provision of paging.</returns>
    /// <exception cref="NotImplementedException">Thrown when the Translator returns a <c>null</c> result, which
    /// represents an invalid request for the Translator and is not currently supported.</exception>
    public async Task<IActionResult> Details(TranslateViewModel viewModel)
    {
        DateTime startTimeStamp = DateTime.Now;
        AssertValidTranslators(viewModel.Translators!);
        
        ITranslatorFactory translatorFactory = await GetTranslatorFactory(viewModel.Translators!.Single());

        ITranslatorConfiguration translatorConfiguration =
            await _translatorConfigurationCache.GetTranslatorConfiguration(translatorFactory.GetTranslatorName(),_translatorConfigurationRepository);
        
        ITranslator translator=translatorFactory.Create(_dataContext,translatorConfiguration);

        TranslationRequest translationRequest = TranslationRequestUtility.CreateTranslationRequestFromTranslateViewModel(viewModel,null);
        translationRequest.DataWillBePaged = true;
        
        TranslationResults translationResults = translator.Translate(translationRequest);
        if (translationResults.ResultItems == null)
            throw new NotImplementedException("Translator reported a null result");
        
        // translation results for summaries are truncated and the paginator is not displayed
        int maximumNumberOfItemsToDisplayPerPage = 10; // TODO this should be a configuration
        translationResults.MaximumItems = maximumNumberOfItemsToDisplayPerPage < translationResults.ResultItems.Count() ? maximumNumberOfItemsToDisplayPerPage : translationResults.ResultItems.Count(); 
        
        // generate pagination data
        int numberOfPossiblePages =
            (translationResults.NumberOfAvailableItemsBeforePaging ?? 1) / maximumNumberOfItemsToDisplayPerPage;
        if (translationResults.NumberOfAvailableItemsBeforePaging.HasValue && translationResults.NumberOfAvailableItemsBeforePaging % maximumNumberOfItemsToDisplayPerPage > 0)
            numberOfPossiblePages++;

        int currentPageNumber = (translationRequest.OrdinalOfFirstResult / maximumNumberOfItemsToDisplayPerPage) + 1;
        
        TimeSpan delta = DateTime.Now - startTimeStamp;
        TranslationResultsWithMetaData translationResultsWithMetaData =
            new TranslationResultsWithMetaData(translationResults,translationRequest)
            {
                Translator = translator.GetType().Name,
                TimeTaken = delta,
                JobId = null,
                IsRenderedAsDetailsView = true
            }; 
        
        
        string viewName = $"Views\\Translate\\{translator.GetType().Name}.cshtml";
        string renderedView = await RazorViewRendererFactory.Create(
            _webHostEnvironment.ContentRootPath,
            _webHostEnvironment.WebRootPath,
            "Taggloo4.Web").RenderAsync(viewName, translationResultsWithMetaData);
            
        DetailsViewModel detailsViewModel = new DetailsViewModel()
        {
            TranslationResultsWithMetaData = translationResultsWithMetaData,
            RenderedSubView = renderedView,
            CurrentPageNumber = currentPageNumber,
            NumberOfPages = numberOfPossiblePages,
            NumberOfItemsPerPage = maximumNumberOfItemsToDisplayPerPage
        };
        
        return View(detailsViewModel);
    }

    private ITranslatorFactory GetTranslatorFactory(string translatorKey)
    {
        // have to pass the ITranslatorRepository because it is scoped
        IEnumerable<ITranslatorFactory> translatorFactories =_translationFactoryService.GetTranslatorFactories(_translatorRepository);

        ITranslatorFactory? matchingTranslator = translatorFactories.SingleOrDefault(q =>
            q.GetTranslatorName().Equals(translatorKey, StringComparison.OrdinalIgnoreCase));

        if (matchingTranslator == null)
            throw new InvalidOperationException($"Translator '{translatorKey}' could not be resolved");
        
        return matchingTranslator;
    }


    private void AssertValidTranslators(IEnumerable<string> translators)
    {
        if (translators == null) throw new ArgumentNullException(nameof(translators));
        if (!translators.Any()) throw new InvalidOperationException("Translators cannot be empty");
        if (translators.Count() > 1) throw new InvalidOperationException("Only a single Translator is supported");
    }
}