using API.ViewModels.Home;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.ViewModels.Translate;

public class TranslateViewModel : ITranslationRequestViewModel
{
    public string TranslationId { get; set; }
    
    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? Query { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? FromLanguageCode { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? ToLanguageCode { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public IEnumerable<SelectListItem>? AllLanguages { get; set; }
    
}