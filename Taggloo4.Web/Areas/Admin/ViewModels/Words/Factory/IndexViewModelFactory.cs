using Taggloo4.Model;
using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Words.Factory;

public class IndexViewModelFactory : IViewModelFactory<IndexViewModel>
{
    private readonly IEnumerable<WordInDictionary> _wordsInDictionary;
    private readonly int _currentPageNumber;
    private readonly int _numberOfPages;
    private readonly int _totalUnpagedItems;
    private readonly int _itemsPerPage;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public IndexViewModelFactory(IEnumerable<WordInDictionary> wordsInDictionary, int currentPageNumber,
        int numberOfPages, int totalUnpagedItems, int itemsPerPage)
    {
        _wordsInDictionary = wordsInDictionary;
        _currentPageNumber = currentPageNumber;
        _numberOfPages = numberOfPages;
        _totalUnpagedItems = totalUnpagedItems;
        _itemsPerPage = itemsPerPage;
    }

    /// <inheritdoc cref="IViewModelFactory{TViewModelType}"/>
    public IndexViewModel Create()
    {
        IndexViewModel viewModel = new IndexViewModel();
        Configure(ref viewModel);

        return viewModel;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref IndexViewModel viewModel)
    {
        viewModel.Results = _wordsInDictionary;
        viewModel.CurrentPageNumber = _currentPageNumber;
        viewModel.NumberOfPages = _numberOfPages;
        viewModel.TotalUnpagedItems = _totalUnpagedItems;
        viewModel.ItemsPerPage = _itemsPerPage;
    }
}
