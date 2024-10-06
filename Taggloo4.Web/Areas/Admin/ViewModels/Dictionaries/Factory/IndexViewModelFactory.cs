using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries.Factory;

/// <summary>
/// Factory for constructing a configured <seealso cref="Words.IndexViewModel"/>.
/// </summary>
public class IndexViewModelFactory : IViewModelFactory<IndexViewModel>
{
    private readonly IEnumerable<DictionaryWithContentTypeAndLanguage> _dictionariesWithContentTypeAndLanguage;
    private readonly int _currentPageNumber;
    private readonly int _numberOfPages;
    private readonly int _totalUnpagedItems;
    private readonly int _itemsPerPage;
    private readonly DictionariesSortColumn? _sortBy;
    private readonly SortDirection? _sortDirection;
    private readonly string? _query;
    private readonly string? _ietfLanguageTag;
    private readonly int? _contentTypeId;
    private readonly IEnumerable<Language> _allLanguages;
    private readonly IEnumerable<ContentType> _allContentTypes;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public IndexViewModelFactory(IEnumerable<DictionaryWithContentTypeAndLanguage> dictionariesWithContentTypeAndLanguage, int currentPageNumber,
        int numberOfPages, int totalUnpagedItems, int itemsPerPage, DictionariesSortColumn? sortBy, SortDirection? sortDirection,
        string? query,
        string? ietfLanguageTag, int? contentTypeId, IEnumerable<Language> allLanguages, IEnumerable<ContentType> allContentTypes)
    {
        _dictionariesWithContentTypeAndLanguage = dictionariesWithContentTypeAndLanguage;
        _currentPageNumber = currentPageNumber;
        _numberOfPages = numberOfPages;
        _totalUnpagedItems = totalUnpagedItems;
        _itemsPerPage = itemsPerPage;
        _sortBy = sortBy;
        _sortDirection = sortDirection;
        _query = query;
        _ietfLanguageTag = ietfLanguageTag;
        _contentTypeId = contentTypeId;
        _allLanguages = allLanguages;
        _allContentTypes = allContentTypes;
        
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
        viewModel.Results = _dictionariesWithContentTypeAndLanguage;
        viewModel.CurrentPageNumber = _currentPageNumber;
        viewModel.NumberOfPages = _numberOfPages;
        viewModel.TotalUnpagedItems = _totalUnpagedItems;
        viewModel.ItemsPerPage = _itemsPerPage;
        viewModel.SortBy = _sortBy ?? DictionariesSortColumn.DictionaryId;
        viewModel.SortDirection = _sortDirection ?? SortDirection.Ascending;
        viewModel.AllContentTypeOptions = new[]
        {
            new SelectListItem("(all content types)","")
        }.Union(_allContentTypes.Select(q=>new SelectListItem(q.NamePlural,q.Id.ToString())));
        viewModel.AllLanguagesOptions = new[]
        {
            new SelectListItem("(all Languages)","")
        }.Union(_allLanguages.Select(q=>new SelectListItem(q.Name,q.IetfLanguageTag))).ToArray();
        viewModel.IetfLanguageTag = _ietfLanguageTag;
        viewModel.Query = _query;
        viewModel.ContentType = _contentTypeId;
    }
}
