using API.Hubs;
using API.Translation;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace API.Jobs;

public class TranslateJob 
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;
    
    public TranslateJob(IBackgroundJobClient backgroundJobClient, IHubContext<TranslateHub> hubContext)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
    }

    public void AddTranslationJob(TranslationRequest translationRequest) 
    {
        string? jobId = _backgroundJobClient.Enqueue<TranslateJob>(x => x.ProcessTranslationJob(translationRequest));
        
        // each translator gets a continueWith
        _backgroundJobClient.ContinueJobWith<TranslateJob>(jobId, x => x.PublishTranslationResultsAsync(translationRequest, jobId));
    }

    public async Task ProcessTranslationJob(TranslationRequest translationRequest)
    {
        // basic meta data for translation
    }

    public async Task PublishTranslationResultsAsync(TranslationRequest translationRequest, string hangfireJobId)
    {
        // this will be called per translator
        
        // no results yet, return
        
        // have results
        await _hubContext.Clients.Client(translationRequest.ClientId).SendCoreAsync("UpdateTranslationResults", new[] 
        {
            translationRequest.Query
        });
    }
}