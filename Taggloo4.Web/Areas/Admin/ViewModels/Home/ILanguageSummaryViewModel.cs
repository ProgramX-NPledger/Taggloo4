using Taggloo4.Web.Areas.Admin.Models.DTOs;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Home;

/// <summary>
/// Summary information for Languages.
/// </summary>
public interface ILanguageSummaryViewModel
{
    /// <summary>
    /// All Languages configured in Taggloo.
    /// </summary>
    IEnumerable<LanguageDto>? AllLanguages { get; set; }
}