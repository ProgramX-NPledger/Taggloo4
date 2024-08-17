using System.Text.Json.Serialization;
using API.ViewModels.Home;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.ViewModels.Translate;

public class TranslateViewModel : ITranslationRequestViewModel
{
    public string TranslationId { get; set; }
    
    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    [JsonPropertyName("fromLanguageCode")]
    public string? FromLanguageCode { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    [JsonPropertyName("toLanguageCode")]
    public string? ToLanguageCode { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public IEnumerable<SelectListItem>? AllLanguages { get; set; }
    
}