using API.Hubs;
using API.Jobs;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace API.Translation;

public class Translator
{
    private readonly TranslatorOptions _options;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;

    public Translator(IBackgroundJobClient backgroundJobClient, IHubContext<TranslateHub> hubContext) // TODO: investigate other ways of passing in the SignalR hub context
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
    }

    public Translator(IBackgroundJobClient backgroundJobClient, IHubContext<TranslateHub> hubContext, TranslatorOptions options) : this(backgroundJobClient, hubContext)
    {
        _options = options;
    }

    public string Translate(TranslationRequest translationRequest)
    {
        if (_backgroundJobClient == null)
            throw new InvalidOperationException("BackgroundJobClient implementation cannot be null");
        
        Guid translationGuid = Guid.NewGuid();

        // create the hangfire job and submit
        TranslateJob translateJob = new TranslateJob(_backgroundJobClient, _hubContext);
        string? jobId=translateJob.AddTranslationJob(translationRequest);
        
        return $"{translationGuid}:{jobId ?? "null"}";
    }
}