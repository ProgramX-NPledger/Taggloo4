using API.Translation;

namespace API.ViewModels.Translate;

public class DetailsViewModel
{
    public TranslationResultsWithMetaData TranslationResultsWithMetaData { get; set; }

    public string RenderedSubView { get; set; }

    public int CurrentPageNumber { get; set; } = 1;

    public int? NumberOfPages { get; set; }

    public int? NumberOfItemsPerPage { get; set; }
    

}