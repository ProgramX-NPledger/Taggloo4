using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Contract;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Utility;
using Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries;
using Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries.Factory;
using Taggloo4.Web.Constants;

namespace Taggloo4.Web.Areas.Admin.Controllers;

/// <summary>
/// Controller for Admin area actions.
/// </summary>
/// <remarks>
/// This Controller explicitly sets the desired AuthenticationScheme to avoid conflict with JWT authentication.
/// </remarks>
[Authorize(AuthenticationSchemes = "Identity.Application")]
[Area("Admin")]
[Authorize(Roles = "administrator")]
public class DictionariesController : Controller
{
    private readonly IWordRepository _wordRepository;
    private readonly IDictionaryRepository _dictionaryRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default constructor with injected properties.
    /// </summary>
    /// <param name="wordRepository">Implementation of <seealso cref="IWordRepository"/>.</param>
    /// <param name="dictionaryRepository">Implementation of <seealso cref="IDictionaryRepository"/>.</param>
    /// <param name="languageRepository">Implementation of <seealso cref="ILanguageRepository"/>.</param>
    /// <param name="configuration">Implementation of ASP.NET configuration API.</param>
    public DictionariesController(IWordRepository wordRepository, IDictionaryRepository dictionaryRepository, ILanguageRepository languageRepository, IConfiguration configuration)
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
    public async Task<IActionResult> Index(int? from, int? itemsPerPage, string? query, string? ietfLanguageTag, string? contentType, DictionariesSortColumn? sortBy, SortDirection? sortDirection)
    {
        int ordinalOfFirstItem = from ?? 0;
        int maximumItems = itemsPerPage ?? _configuration.GetValue<int?>("DefaultPageSize") ?? SearchAndPages.MaximumItemsPerPage;
        if (itemsPerPage>SearchAndPages.MaximumItemsPerPage) maximumItems = SearchAndPages.MaximumItemsPerPage;

        int? contentTypeId = TypeConverter.ConvertNullableStringToNullableInt(contentType);
        
        PagedResults<DictionaryWithContentTypeAndLanguage> results = await _dictionaryRepository.GetDictionariesByCriteriaAsync(new GetDictionariesCriteria()
        {
            SortDirection = sortDirection ?? SortDirection.Ascending,
            ContentTypeId = contentTypeId,
            Query = query,
            SortBy = sortBy ?? DictionariesSortColumn.DictionaryId,
            ItemsPerPage = maximumItems,
            OrdinalOfFirstItem = ordinalOfFirstItem,
            IetfLanguageTag = ietfLanguageTag
        });

        int currentPageNumber = (ordinalOfFirstItem+1) / maximumItems;
        currentPageNumber++;
        int numberOfPages = results.TotalUnpagedItems / maximumItems;
        if (results.TotalUnpagedItems % maximumItems>0) numberOfPages++;

        IEnumerable<Language> allLanguages = await _languageRepository.GetAllLanguagesAsync();
        IEnumerable<ContentType> allContentTypes = await _dictionaryRepository.GetAllContentTypesAsync();

        IndexViewModelFactory viewModelFactory = new IndexViewModelFactory(results.Results, currentPageNumber,
            numberOfPages, results.TotalUnpagedItems, maximumItems, sortBy, sortDirection, query,ietfLanguageTag, contentTypeId, allLanguages, allContentTypes);
        IndexViewModel viewModel = viewModelFactory.Create();
        return View(viewModel);
        
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return BadRequest();
        
        Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(id.Value);
        if (dictionary == null) return NotFound();
    
        DetailsViewModelFactory viewModelFactory = new DetailsViewModelFactory(dictionary);
        DetailsViewModel viewModel = viewModelFactory.Create();
        return View(viewModel);
    
    }
    
}