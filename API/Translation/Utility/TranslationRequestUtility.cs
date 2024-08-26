using API.ViewModels.Translate;

namespace API.Translation.Utility;

public static class TranslationRequestUtility
{
    public static TranslationRequest CreateTranslationRequestFromTranslateViewModel(TranslateViewModel viewModel, string? signalRConnectionId)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Query)) throw new InvalidOperationException("Query cannot be null");
        if (string.IsNullOrWhiteSpace(viewModel.FromLanguageCode))
            throw new InvalidOperationException("FromLanguageCode cannot be null");
        if (string.IsNullOrWhiteSpace(viewModel.ToLanguageCode))
            throw new InvalidOperationException("ToLanguageCode cannot be null");
        
        TranslationRequest translationRequest = new TranslationRequest()
        {
            Query = viewModel.Query,
            ClientId = signalRConnectionId,
            FromLanguageCode = viewModel.FromLanguageCode,
            ToLanguageCode = viewModel.ToLanguageCode,
            MaximumNumberOfResults = viewModel.MaximumResults,
            OrdinalOfFirstResult = viewModel.OrdinalOfFirstResult
        };
        return translationRequest;
    }
}