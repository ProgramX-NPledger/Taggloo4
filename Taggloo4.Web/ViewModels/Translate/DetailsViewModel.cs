using Taggloo4.Contract.Translation;
using Taggloo4.Web.Translation;

namespace Taggloo4.Web.ViewModels.Translate;

/// <summary>
/// Details of a Translation View Model. The Details view focuses on a particular <seealso cref="ITranslator"/>'s results and provides paging functionality.
/// </summary>
public class DetailsViewModel
{
    /// <summary>
    /// Augmented results from the <seealso cref="ITranslator"/>.
    /// </summary>
    public required TranslationResultsWithMetaData TranslationResultsWithMetaData { get; set; }

    /// <summary>
    /// The rendered sub-view as a string, which will be included in the output.
    /// </summary>
    public required string RenderedSubView { get; set; }

    /// <summary>
    /// Current page number.
    /// </summary>
    public int CurrentPageNumber { get; set; } = 1;

    /// <summary>
    /// Number of pages that are available for viewing.
    /// </summary>
    public int? NumberOfPages { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int? NumberOfItemsPerPage { get; set; }
    

}