using System.ComponentModel.DataAnnotations;
using API.ViewModels.Home;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.Model.Home;

/// <summary>
/// Defines the View Model for the Home/Index view, including dependent View Models for partial Views.
/// </summary>
public class IndexViewModel : ILargeTranslateFormViewModel
{
    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? Query { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? FromLanguageCode { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public string? ToLanguageCode { get; set; }

    /// <inheritdoc cref="ILargeTranslateFormViewModel"/>
    public IEnumerable<SelectListItem>? AllLanguages { get; set; }
}