using API.Hubs;
using API.Translation;
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
    
    /// <summary>
    /// Constructor for purposes of injecting required services.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of Hangfire <seealso cref="IBackgroundJobClient"/>.</param>
    /// <param name="hubContext">Implementation of SignalR <seealso cref="IHubContext"/>.</param>
    public TranslateJob(IBackgroundJobClient backgroundJobClient, IHubContext<TranslateHub> hubContext)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Add a Translation Job to the queue for processing. 
    /// </summary>
    /// <param name="translationRequest"></param>
    public void AddTranslationJob(TranslationRequest translationRequest) 
    {
        string? jobId = _backgroundJobClient.Enqueue<TranslateJob>(x => x.ProcessTranslationJob(translationRequest));
        
        // each translator gets a continueWith
        _backgroundJobClient.ContinueJobWith<TranslateJob>(jobId, x => x.PublishTranslationResultsAsync(translationRequest, jobId));
    }

    public void ProcessTranslationJob(TranslationRequest translationRequest)
    {
        // basic meta data for translation
    }

    public async Task PublishTranslationResultsAsync(TranslationRequest translationRequest, string hangfireJobId)
    {
        // this will be called per translator
        
        // no results yet, return
        
        // have results
        if (!string.IsNullOrWhiteSpace(translationRequest.ClientId))
        {
            await _hubContext.Clients.Client(translationRequest.ClientId).SendCoreAsync("UpdateTranslationResults", new[] 
            {
                translationRequest.Query
            });
            
        }
    }
}