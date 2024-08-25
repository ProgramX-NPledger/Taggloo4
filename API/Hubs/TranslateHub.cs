using System.Text.Json;
using API.Data;
using API.Translation;
using API.Translation.Utility;
using API.ViewModels.Translate;
using Hangfire;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

/// <summary>
/// SignalR Hub for purposes of handling communication to/from clients.
/// </summary>
public class TranslateHub : Hub
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;
    private readonly DataContext _entityFrameworkCoreDataContext;
    private readonly IWebHostEnvironment _webHosEnvironment;

    private readonly ILogger<TranslateHub> _logger;
    // private readonly ICompositeViewEngine _compositeViewEngine;
    // private readonly ITempDataProvider _tempDataProvider;
    // private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructor for injection of required services.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of Hangfire <seealso cref="IBackgroundJobClient"/>.</param>
    /// <param name="hubContext">Implementation of SignalR <seealso cref="IHubContext"/>.</param>
    /// <param name="entityFrameworkCoreDataContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <param name="webHostEnvironment">Implementation of ASP.NET <seealso cref="IWebHostEnvironment"/>.</param>
    public TranslateHub(IBackgroundJobClient backgroundJobClient,
        IHubContext<TranslateHub> hubContext,
        DataContext entityFrameworkCoreDataContext,
        IWebHostEnvironment webHostEnvironment,
        ILogger<TranslateHub> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
        _entityFrameworkCoreDataContext = entityFrameworkCoreDataContext;
        _webHosEnvironment = webHostEnvironment;
        _logger = logger;
    }

    /// <summary>
    /// Invoked from the client, this will create a Translation Request, including the client's unique identifier
    /// to allow for updating on the job progress as it completes.
    /// </summary>
    /// <param name="args">Arguments from the client. The only element must be serializable into a <seealso cref="TranslateViewModel"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown if the client request was invalid.</exception>
    public void InvokeTranslation(object[] args)
    {
        // the first element in the arguments must be an instance of the ITranslationRequestViewModel
        if (args.Length < 1) throw new InvalidOperationException($"Invalid arguments count");
        if (!(args[0] is JsonElement)) throw new InvalidOperationException("Invalid argument");
        JsonElement jsonElement = (JsonElement)args[0];
        TranslateViewModel? translateViewModel = JsonSerializer.Deserialize<TranslateViewModel>(jsonElement.ToString());
        if (translateViewModel == null)
            throw new InvalidOperationException($"Failed to deserialize {nameof(TranslateViewModel)}");

        try
        {
            TranslationRequest translationRequest = TranslationRequestUtility.CreateTranslationRequestFromTranslateViewModel(translateViewModel, Context.ConnectionId);
        
            // start the translation by using the Translator object, which schedules on the Hangfire background job client
            // having a single Translator class allows for multiple entrypoints/clients to implement translation
            AsynchronousTranslatorSession translator = new AsynchronousTranslatorSession(_backgroundJobClient, _hubContext, _entityFrameworkCoreDataContext, _webHosEnvironment);
            translator.Translate(translationRequest);
        }
        catch (InvalidOperationException ioEx)
        {
            // an invalid JSON request was made, so no submission is made 
            _logger.LogError("An invalid JSON request was made");
        }
    
    }
    
    
    
}