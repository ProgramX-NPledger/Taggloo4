using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Words;

/// <summary>
/// View-Model for the Words primary page.
/// </summary>
public class IndexViewModel
{
    /// <summary>
    /// Paged results.
    /// </summary>
    public IEnumerable<WordInDictionary> Results { get; set; }
    
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
    public WordsSortColumn SortBy { get; set; }

    /// <summary>
    /// Sorting direction for the selected <seealso cref="SortBy"/> column.
    /// </summary>
    public SortDirection SortDirection { get; set; }

    /// <summary>
    /// All available Language options for selection for filtering.
    /// </summary>
    public IEnumerable<SelectListItem> AllLanguagesOptions { get; set; }

    /// <summary>
    /// All available Dictionary options for selection for filtering.
    /// </summary>
    public IEnumerable<SelectListItem> AllDictionariesOptions { get; set; }

    /// <summary>
    /// When specified, allows filtering by Dictionary.
    /// </summary>
    public int? DictionaryId { get; set; }

    /// <summary>
    /// When specified, allows filtering by Language.
    /// </summary>
    public string? IetfLanguageTag { get; set; }

    /// <summary>
    /// Free-form text.
    /// </summary>
    public string? Query { get; set; }
    
    
    
}