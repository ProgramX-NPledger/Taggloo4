using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
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
    private readonly WordsSortColumn? _sortBy;
    private readonly SortDirection? _sortDirection;
    private readonly string? _query;
    private readonly string? _ietfLanguageTag;
    private readonly IEnumerable<Language> _allLanguages;
    private readonly int? _dictionaryId;
    private readonly IEnumerable<Dictionary> _allDictionaries;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public IndexViewModelFactory(IEnumerable<WordInDictionary> wordsInDictionary, int currentPageNumber,
        int numberOfPages, int totalUnpagedItems, int itemsPerPage, WordsSortColumn? sortBy, SortDirection? sortDirection,
        string? query,
        string? ietfLanguageTag, IEnumerable<Language> allLanguages, 
        int? dictionaryId, IEnumerable<Dictionary> allDictionaries)
    {
        _wordsInDictionary = wordsInDictionary;
        _currentPageNumber = currentPageNumber;
        _numberOfPages = numberOfPages;
        _totalUnpagedItems = totalUnpagedItems;
        _itemsPerPage = itemsPerPage;
        _sortBy = sortBy;
        _sortDirection = sortDirection;
        _query = query;
        _ietfLanguageTag = ietfLanguageTag;
        _allLanguages = allLanguages;
        _dictionaryId = dictionaryId;
        _allDictionaries = allDictionaries;
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
        viewModel.SortBy = _sortBy ?? WordsSortColumn.WordId;
        viewModel.SortDirection = _sortDirection ?? SortDirection.Ascending;
        viewModel.AllDictionariesOptions = new[]
        {
            new SelectListItem("(all Dictionaries)","")
        }.Union(_allDictionaries.Select(q=>new SelectListItem(q.Name,q.Id.ToString()))).ToArray();
        viewModel.AllLanguagesOptions = new[]
        {
            new SelectListItem("(all Languages)","")
        }.Union(_allLanguages.Select(q=>new SelectListItem(q.Name,q.IetfLanguageTag))).ToArray();
        viewModel.IetfLanguageTag = _ietfLanguageTag;
        viewModel.DictionaryId = _dictionaryId;
        viewModel.Query = _query;
    }
}
