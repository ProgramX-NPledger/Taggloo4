using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Data;
using Taggloo4.Web.Hubs;
using Taggloo4.Web.RazorViewRendering;
using Taggloo4.Web.Translation;
using Taggloo4.Web.Translation.Translators.Factories;

namespace Taggloo4.Web.Jobs;

/// <summary>
/// A Hangfire Job instantiated per translationr request, providing asynchronous user-experience.
/// </summary>
public class TranslateJob 
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;
    private readonly DataContext _dataContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly TranslatorConfigurationCache _translatorConfigurationCache;
    private readonly ITranslatorConfigurationRepository _translatorConfigurationRepository;

    /// <summary>
    /// Constructor for purposes of injecting required services.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of Hangfire <seealso cref="IBackgroundJobClient"/>.</param>
    /// <param name="hubContext">Implementation of SignalR <seealso cref="IHubContext"/>.</param>
    /// <param name="dataContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <param name="webHostEnvironment">Implementation of ASP.NET <seealso cref="IWebHostEnvironment"/>.</param>
    /// <param name="translatorConfigurationCache">Translator configuration cache.</param>
    /// <param name="translatorConfigurationRepository">Implementation of <seealso cref="ITranslatorConfigurationRepository"/>.</param>
    public TranslateJob(IBackgroundJobClient backgroundJobClient, 
        IHubContext<TranslateHub> hubContext, 
        DataContext dataContext,
        IWebHostEnvironment webHostEnvironment,
        TranslatorConfigurationCache translatorConfigurationCache,
        ITranslatorConfigurationRepository translatorConfigurationRepository)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
        _dataContext = dataContext;
        _webHostEnvironment = webHostEnvironment;
        _translatorConfigurationCache = translatorConfigurationCache;
        _translatorConfigurationRepository = translatorConfigurationRepository;
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
        // TODO: see TranslateController.GetTranslatorFactoryAsync
        return new ITranslatorFactory[]
        {
            new WordTranslatorFactory(),
            new PhraseTranslatorFactory()
        };
    }

    /// <summary>
    /// Called by Hangfire once for each translation request.
    /// </summary>
    /// <param name="translationRequest">The complete Translation request from the client.</param>
    public void ProcessTranslationJob(TranslationRequest translationRequest)
    {
        // basic meta data for translation
    }

    /// <summary>
    /// Called by Hangfire per implementation of <seealso cref="ITranslator"/>, performs translation and returns the result by compiling a Razor view and sending that
    /// view back to the client using SignalR.
    /// </summary>
    /// <param name="translatorFactory">Implementation of <seealso cref="ITranslatorFactory"/> to allow construction of each <seealso cref="ITranslator"/>.</param>
    /// <param name="translationRequest">The complete Translation request from the client.</param>
    /// <param name="hangfireJobId">The job identifier assigned to the Translation task by Hangfire.</param>
    public async Task PublishTranslationResultsAsync(ITranslatorFactory translatorFactory, TranslationRequest translationRequest,
        string hangfireJobId)
    {
        // this will be called per translator
        DateTime startTimeStamp = DateTime.Now;

        ITranslatorConfiguration translatorConfiguration =
            await _translatorConfigurationCache.GetTranslatorConfiguration(translatorFactory.GetTranslatorName(),_translatorConfigurationRepository);
        ITranslator translator = translatorFactory.Create(_dataContext,translatorConfiguration);
        TranslationResults translationResults = translator.Translate(translationRequest);
        // translation results for summaries are truncated and the paginator is not displayed
        translationResults.MaximumItems = translatorConfiguration.NumberOfItemsInSummary;
        
        TimeSpan delta = DateTime.Now - startTimeStamp;
        TranslationResultsWithMetaData translationResultsWithMetaData =
            new TranslationResultsWithMetaData(translationResults,translationRequest)
            {
                Translator = translator.GetType().Name,
                TimeTaken = delta,
                JobId = hangfireJobId,
                Priority = translatorConfiguration.Priority
            }; 
        
        if (!string.IsNullOrWhiteSpace(translationRequest.ClientId))
        {
            string viewName = $"Views\\Translate\\{translator.GetType().Name}.cshtml";
            string renderedView = await RazorViewRendererFactory.Create(
                _webHostEnvironment.ContentRootPath,
                _webHostEnvironment.WebRootPath,
                "Taggloo4.Web"
            ).RenderAsync(viewName, translationResultsWithMetaData);
            
            await _hubContext.Clients.Client(translationRequest.ClientId).SendCoreAsync("UpdateTranslationResults", new[] 
            {
                translator.GetType().Name,
                translatorConfiguration.Priority.ToString(),
                renderedView
            });
        }
    }
 
}