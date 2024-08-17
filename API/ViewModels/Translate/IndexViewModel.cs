using System.ComponentModel.DataAnnotations;
using API.ViewModels.Home;
using API.ViewModels.Translate;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.ViewModels.Translate;

/// <summary>
/// Defines the View Model for the Translate/Index view, including dependent View Models for partial Views.
/// </summary>
public class IndexViewModel : ITranslationRequestViewModel
{

    #region ~ from ILargeTranslateFormViewModel ~
    
    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? Query { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? FromLanguageCode { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? ToLanguageCode { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public IEnumerable<SelectListItem>? AllLanguages { get; set; }
    
    #endregion
    
    
}