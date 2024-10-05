using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Web.Areas.Admin.Models.DTOs;
using Taggloo4.Web.Areas.Admin.ViewModels.Home;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries;

/// <summary>
/// Defines the View Model for the Admin/Home/Index view, including dependent View Models for partial Views.
/// </summary>
public class IndexViewModel 
{
    /// <summary>
    /// Paged results.
    /// </summary>
    public IEnumerable<DictionaryWithContentTypeAndLanguage> Results { get; set; } = [];
    
    /// <summary>
    /// The currently selected page.
    /// </summary>
    public int CurrentPageNumber { get; set; }
    
    /// <summary>
    /// Total number of available pages.
    /// </summary>
    public int NumberOfPages { get; set; }

    /// <summary>
    /// Total number of items before paging is applied.
    /// </summary>
    public int TotalUnpagedItems { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int ItemsPerPage { get; set; }

    /// <summary>
    /// Column for sorting by.
    /// </summary>
    public DictionariesSortColumn SortBy { get; set; }

    /// <summary>
    /// Sorting direction for the selected <seealso cref="SortBy"/> column.
    /// </summary>
    public SortDirection SortDirection { get; set; }

    /// <summary>
    /// All available Language options for selection for filtering.
    /// </summary>
    public IEnumerable<SelectListItem> AllLanguagesOptions { get; set; } = [];

    /// <summary>
    /// All available Content Type options for selection for filtering.
    /// </summary>
    public IEnumerable<SelectListItem> AllContentTypeOptions { get; set; } = [];

    /// <summary>
    /// When specified, allows filtering by Language.
    /// </summary>
    public string? IetfLanguageTag { get; set; }

    /// <summary>
    /// Free-form text.
    /// </summary>
    public string? Query { get; set; }

}