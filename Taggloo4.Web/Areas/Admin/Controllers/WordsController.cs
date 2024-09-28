using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.QuickInfo;
using Taggloo4.Contract;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Web.Areas.Admin.Models;
using Taggloo4.Web.Areas.Admin.ViewModels.Words;
using Taggloo4.Web.Areas.Admin.ViewModels.Words.Factory;
using Taggloo4.Web.Constants;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Controllers.Admin;

/// <summary>
/// Controller for Admin area actions.
/// </summary>
/// <remarks>
/// This Controller explicitly sets the desired AuthenticationScheme to avoid conflict with JWT authentication.
/// </remarks>
[Authorize(AuthenticationSchemes = "Identity.Application")]
[Area("Admin")]
[Authorize(Roles = "administrator")]
public class WordsController : Controller
{
    private readonly IWordRepository _wordRepository;
    private readonly IDictionaryRepository _dictionaryRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default constructor with injected properties.
    /// </summary>
    /// <param name="wordRepository">Implementation of <seealso cref="IWordRepository"/>.</param>
    public WordsController(IWordRepository wordRepository, IDictionaryRepository dictionaryRepository, ILanguageRepository languageRepository, IConfiguration configuration)
    {
        _wordRepository = wordRepository;
        _dictionaryRepository = dictionaryRepository;
        _languageRepository = languageRepository;
        _configuration = configuration;
    }
    
    /// <summary>
    /// Default page with paged grid and filters.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index(int? from, int? itemsPerPage, int? dictionaryId, string? query, string? ietfLanguageTag, WordsSortColumn? sortBy, SortDirection? sortDirection)
    {
        int ordinalOfFirstItem = from ?? 0;
        int maximumItems = itemsPerPage ?? _configuration.GetValue<int?>("DefaultPageSize") ?? SearchAndPages.MaximumItemsPerPage;
        if (itemsPerPage>SearchAndPages.MaximumItemsPerPage) maximumItems = SearchAndPages.MaximumItemsPerPage;

        PagedResults<WordInDictionary> results = await _wordRepository.GetWordsByCriteriaAsync(new GetWordsCriteria()
        {
            SortDirection = sortDirection ?? SortDirection.Ascending,
            DictionaryId = dictionaryId,
            Query = query,
            SortBy = sortBy ?? WordsSortColumn.WordId,
            ItemsPerPage = maximumItems,
            OrdinalOfFirstItem = ordinalOfFirstItem,
            IetfLanguageTag = ietfLanguageTag
        });

        int currentPageNumber = (ordinalOfFirstItem+1) / maximumItems;
        currentPageNumber++;
        int numberOfPages = results.TotalUnpagedItems / maximumItems;
        if (results.TotalUnpagedItems % maximumItems>0) numberOfPages++;

        IEnumerable<Language> allLanguages = await _languageRepository.GetAllLanguagesAsync();
        IEnumerable<Dictionary> allDictionaries = await _dictionaryRepository.GetDictionariesAsync(null,null);

        IndexViewModelFactory viewModelFactory = new IndexViewModelFactory(results.Results, currentPageNumber,
            numberOfPages, results.TotalUnpagedItems, maximumItems, sortBy, sortDirection, query,ietfLanguageTag, allLanguages, dictionaryId, allDictionaries);
        IndexViewModel viewModel = viewModelFactory.Create();
        return View(viewModel);
        
    }
    
}