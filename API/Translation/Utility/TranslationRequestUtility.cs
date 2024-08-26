using API.ViewModels.Translate;

namespace API.Translation.Utility;

/// <summary>
/// Utility class for translaiton requests.
/// </summary>
public static class TranslationRequestUtility
{
    /// <summary>
    /// Creates a <seealso cref="TranslationRequest"/> which may be used with an implementation of <seealso cref="ITranslator"/>.
    /// </summary>
    /// <param name="viewModel">The view model having been bound.</param>
    /// <param name="signalRConnectionId">If this is a SignalR communication, the identifier of the client.</param>
    /// <returns>A <seealso cref="TranslationRequest"/> for use with any <seealso cref="ITranslator"/> implementation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the provided view model is invalid.</exception>
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