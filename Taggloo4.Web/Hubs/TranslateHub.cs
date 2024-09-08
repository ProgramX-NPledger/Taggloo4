using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Data;
using Taggloo4.Web.Translation;
using Taggloo4.Web.Translation.Utility;
using Taggloo4.Web.ViewModels.Translate;

namespace Taggloo4.Web.Hubs;

/// <summary>
/// SignalR Hub for purposes of handling communication to/from clients.
/// </summary>
public class TranslateHub : Hub
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;
    private readonly DataContext _entityFrameworkCoreDataContext;
    private readonly IWebHostEnvironment _webHosEnvironment;
    private readonly TranslatorConfigurationCache _translatorConfigurationCache;
    private readonly ITranslatorConfigurationRepository _translatorConfigurationRepository;

    private readonly ILogger<TranslateHub> _logger;

    /// <summary>
    /// Constructor for injection of required services.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of Hangfire <seealso cref="IBackgroundJobClient"/>.</param>
    /// <param name="hubContext">Implementation of SignalR <seealso cref="IHubContext"/>.</param>
    /// <param name="entityFrameworkCoreDataContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <param name="webHostEnvironment">Implementation of ASP.NET <seealso cref="IWebHostEnvironment"/>.</param>
    /// <param name="translatorConfigurationCache">Translator configuration cache.</param>
    /// <param name="translatorConfigurationRepository">Implementation of <seealso cref="ITranslatorConfigurationRepository"/>.</param>
    /// <param name="logger">Logging provider to allow for logging.</param>
    public TranslateHub(IBackgroundJobClient backgroundJobClient,
        IHubContext<TranslateHub> hubContext,
        DataContext entityFrameworkCoreDataContext,
        IWebHostEnvironment webHostEnvironment,
        TranslatorConfigurationCache translatorConfigurationCache,
        ITranslatorConfigurationRepository translatorConfigurationRepository,
        ILogger<TranslateHub> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
        _entityFrameworkCoreDataContext = entityFrameworkCoreDataContext;
        _webHosEnvironment = webHostEnvironment;
        _translatorConfigurationCache = translatorConfigurationCache;
        _translatorConfigurationRepository = translatorConfigurationRepository;
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
            AsynchronousTranslatorSession translator = new AsynchronousTranslatorSession(_backgroundJobClient, _hubContext, _entityFrameworkCoreDataContext, _webHosEnvironment, _translatorConfigurationCache, _translatorConfigurationRepository);
            translator.Translate(translationRequest);
        }
        catch (InvalidOperationException ioEx)
        {
            // an invalid JSON request was made, so no submission is made 
            _logger.LogError(ioEx,"An invalid JSON request was made");
        }
    
    }
    
    
    
}