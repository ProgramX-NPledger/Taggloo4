using System.Text.Json.Serialization;
using API.ViewModels.Home;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.ViewModels.Translate;

/// <summary>
/// Implementation of fields for Translation requests.
/// </summary>
public class TranslateViewModel : ITranslationRequestViewModel
{
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

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    [JsonPropertyName("ordinalOfFirstResult")]
    public int OrdinalOfFirstResult { get; set; } = 0;

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    [JsonPropertyName("maximumResults")] 
    public int MaximumResults { get; set; } = int.MaxValue;

    /// <summary>
    /// The Translators that should be used
    /// </summary>
    public IEnumerable<string>? Translators { get; set; }
}