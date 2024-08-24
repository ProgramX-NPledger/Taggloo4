using API.Data;
using API.Hubs;
using API.RazorViewRendering;
using API.Translation;
using API.Translation.Translators.Factories;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;

namespace API.Jobs;

/// <summary>
/// A Hangfire Job instantiated per translationr request, providing asynchronous user-experience.
/// </summary>
public class TranslateJob 
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;
    private readonly DataContext _dataContext;
    private readonly ICompositeViewEngine _compositeViewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _webHosEnvironment;

    /// <summary>
    /// Constructor for purposes of injecting required services.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of Hangfire <seealso cref="IBackgroundJobClient"/>.</param>
    /// <param name="hubContext">Implementation of SignalR <seealso cref="IHubContext"/>.</param>
    public TranslateJob(IBackgroundJobClient backgroundJobClient, 
        IHubContext<TranslateHub> hubContext, 
        DataContext dataContext,
        // ICompositeViewEngine compositeViewEngine,
        // ITempDataProvider tempDataProvider,
        // IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHosEnvironment)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
        _dataContext = dataContext;
        // _compositeViewEngine = compositeViewEngine;
        // _tempDataProvider = tempDataProvider;
        // _httpContextAccessor = httpContextAccessor;
        _webHosEnvironment = webHosEnvironment;
    }

    /// <summary>
    /// Add a Translation Job to the queue for processing. 
    /// </summary>
    /// <param name="translationRequest"></param>
    public void AddTranslationJob(TranslationRequest translationRequest) 
    {
        string? jobId = _backgroundJobClient.Enqueue<TranslateJob>(x => x.ProcessTranslationJob(translationRequest));

        IEnumerable<ITranslatorFactory> translatorFactories = GetTranslatorFactories();
        foreach (ITranslatorFactory translatorFactory in translatorFactories)
        {
            // each translator gets a continueWith
            _backgroundJobClient.ContinueJobWith<TranslateJob>(jobId, x => x.PublishTranslationResultsAsync(translatorFactory, translationRequest, jobId));
        }
    }

    private IEnumerable<ITranslatorFactory> GetTranslatorFactories()
    {
        return new[]
        {
            new WordTranslatorFactory()
        };
    }

    public void ProcessTranslationJob(TranslationRequest translationRequest)
    {
        // basic meta data for translation
    }

    public async Task PublishTranslationResultsAsync(ITranslatorFactory translatorFactory, TranslationRequest translationRequest,
        string hangfireJobId)
    {
        // this will be called per translator
        DateTime startTimeStamp = DateTime.Now;
        ITranslator translator = translatorFactory.Create(_dataContext);
        TranslationResults translationResults = translator.Translate(translationRequest);
        TimeSpan delta = DateTime.Now - startTimeStamp;
        TranslationResultsWithMetaData translationResultsWithMetaData =
            new TranslationResultsWithMetaData(translationResults,translationRequest)
            {
                Translator = translator.GetType().Name,
                TimeTaken = delta
            }; // http://localhost:5067/Translate/Translate?Query=thie&FromLanguageCode=gv-GV&ToLanguageCode=en-GB
        
        if (!string.IsNullOrWhiteSpace(translationRequest.ClientId))
        {
            // TODO: Compile view, passing view model and return to client for rendering - no KnockoutJS
            var path = Path.Combine(_webHosEnvironment.ContentRootPath,"Views\\Translate\\TranslatorPartialViews\\WordTranslator.cshtml");
            path = "Views\\Translate\\WordTranslator.cshtml";
            //var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            //var actionContext = new ActionContext(_httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor());
            //viewDataDictionary.Model = translationResultsWithMetaData;
            //var text = await RenderView(path, viewDataDictionary, actionContext);
            


            string renderedView = await RazorViewRendererFactory.New(
                _webHosEnvironment.ContentRootPath,
                _webHosEnvironment.WebRootPath,
                "API"
            ).RenderAsync(path, translationResultsWithMetaData);
            
            
            await _hubContext.Clients.Client(translationRequest.ClientId).SendCoreAsync("UpdateTranslationResults", new[] 
            {
                renderedView
            });
        }
    }
    //
    // private async Task<string> RenderView(string path, ViewDataDictionary viewDataDictionary, ActionContext actionContext)
    // {
    //     using (var sw = new System.IO.StringWriter())
    //     {
    //         var viewResult = _compositeViewEngine.FindView(actionContext, path, false);
    //
    //         TempDataDictionary tempDataDictionary =
    //             new TempDataDictionary(_httpContextAccessor.HttpContext, _tempDataProvider);
    //         HtmlHelperOptions htmlHelperOptions = new HtmlHelperOptions();
    //         var viewContext = new ViewContext(actionContext, viewResult.View, viewDataDictionary, tempDataDictionary, sw, htmlHelperOptions);
    //
    //         await viewResult.View.RenderAsync(viewContext);
    //         sw.Flush();
    //
    //         if (viewContext.ViewData != viewDataDictionary)
    //         {
    //             var keys = viewContext.ViewData.Keys.ToArray();
    //             foreach (var key in keys)
    //             {
    //                 viewDataDictionary[key] = viewContext.ViewData[key];
    //             }
    //         }
    //
    //         return sw.ToString();
    //     }
    // }
}