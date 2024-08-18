using API.Data;
using API.Hubs;
using API.Translation;
using API.Translation.Translators.Factories;
using Hangfire;
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

    /// <summary>
    /// Constructor for purposes of injecting required services.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of Hangfire <seealso cref="IBackgroundJobClient"/>.</param>
    /// <param name="hubContext">Implementation of SignalR <seealso cref="IHubContext"/>.</param>
    public TranslateJob(IBackgroundJobClient backgroundJobClient, IHubContext<TranslateHub> hubContext, DataContext dataContext)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
        _dataContext = dataContext;
    }

    /// <summary>
    /// Add a Translation Job to the queue for processing. 
    /// </summary>
    /// <param name="translationRequest"></param>
    public void AddTranslationJob(TranslationRequest translationRequest) 
    {
        string? jobId = _backgroundJobClient.Enqueue<TranslateJob>(x => x.ProcessTranslationJob(translationRequest));

        IEnumerable<ITranslator> translators = GetTranslators();
        foreach (ITranslator translator in translators)
        {
            // each translator gets a continueWith
            _backgroundJobClient.ContinueJobWith<TranslateJob>(jobId, x => x.PublishTranslationResultsAsync(translator, translationRequest, jobId));
        }
    }

    private IEnumerable<ITranslator> GetTranslators()
    {
        return new[]
        {
            new WordTranslatorFactory().Create(_dataContext)
        };
    }

    public void ProcessTranslationJob(TranslationRequest translationRequest)
    {
        // basic meta data for translation
    }

    public async Task PublishTranslationResultsAsync(ITranslator translator, TranslationRequest translationRequest,
        string hangfireJobId)
    {
        // this will be called per translator
        DateTime startTimeStamp = DateTime.Now;
        TranslationResults translationResults = translator.Translate(translationRequest);
        TimeSpan delta = DateTime.Now - startTimeStamp;
        TranslationResultsWithMetaData translationResultsWithMetaData =
            new TranslationResultsWithMetaData(translationResults,translationRequest)
            {
                Translator = translator.GetType().Name,
                TimeTaken = delta
            };
        
        if (!string.IsNullOrWhiteSpace(translationRequest.ClientId))
        {
            await _hubContext.Clients.Client(translationRequest.ClientId).SendCoreAsync("UpdateTranslationResults", new[] 
            {
                translationResultsWithMetaData
            });
        }
    }
}