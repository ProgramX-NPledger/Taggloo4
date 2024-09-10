using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Taggloo4.Web.ViewModels.Home;

/// <summary>
/// Defines a contract for the use of the <c>_LargeTranslateForm</c> partial view.
/// </summary>
public interface ILargeTranslateFormViewModel
{
    /// <summary>
    /// Query to be entered.
    /// </summary>
    [Microsoft.Build.Framework.Required]
    [MaxLength(256)]
    string? Query { get; set; }

    /// <summary>
    /// From Language (as an IETF Language tag).
    /// </summary>
    [Required]
    string? FromLanguageCode { get; set; }

    /// <summary>
    /// To Langues (as an IETF Language tag).
    /// </summary>
    [Required]
    string? ToLanguageCode { get; set; }

    /// <summary>
    /// List of all supported Languages.
    /// </summary>
    IEnumerable<SelectListItem>? AllLanguages { get; set; }
}