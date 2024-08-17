using System.Text.Json;
using API.Translation;
using API.ViewModels.Translate;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class TranslateHub : Hub
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;

    public TranslateHub(IBackgroundJobClient backgroundJobClient,
        IHubContext<TranslateHub> hubContext)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
    }
    
    public async Task InvokeTranslation(object[] args)
    {
        // this needs to create the translation request, logging the connection id within it
        // to allow the callback to use it to publish the results
        
        // the first element in the arguments must be an instance of the ITranslationRequestViewModel
        if (args.Length < 1) throw new InvalidOperationException($"Invalid arguments count");
        if (!(args[0] is JsonElement)) throw new InvalidOperationException("IInvalid argument");
        JsonElement jsonElement = (JsonElement)args[0];
        TranslateViewModel? translateViewModel =
            (TranslateViewModel)JsonSerializer.Deserialize<TranslateViewModel>(jsonElement.ToString());
        if (translateViewModel == null)
            throw new InvalidOperationException($"Failed to deserialize {nameof(TranslateViewModel)}");
        
        TranslationRequest translationRequest = CreateTranslationRequestFromTranslateViewModel(translateViewModel, Context.ConnectionId);
        
        // start the translation by using the Translator object, which schedules on the Hangfire background job client
        // having a single Translator class allows for multiple entrypoints/clients to implement translation
        Translator translator = new Translator(_backgroundJobClient, _hubContext);
        translator.Translate(translationRequest);
    }
    
    private static TranslationRequest CreateTranslationRequestFromTranslateViewModel(TranslateViewModel viewModel, string signalRConnectionId)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Query)) throw new InvalidOperationException("Query cannot be null");
        
        TranslationRequest translationRequest = new TranslationRequest()
        {
            Query = viewModel.Query,
            ClientId = signalRConnectionId
            
        };
        return translationRequest;
    }
    
}