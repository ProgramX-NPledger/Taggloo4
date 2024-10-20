using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using Taggloo4.Contract;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Utility;
using Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries;
using Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries.Factory;
using Taggloo4.Web.Constants;
using Taggloo4.Web.Hangfire.Jobs;

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
    private readonly IContentTypeRepository _contentTypeRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IConfiguration _configuration;
    private readonly IBackgroundJobClient _backgroundJobClient;

    /// <summary>
    /// Default constructor with injected properties.
    /// </summary>
    /// <param name="wordRepository">Implementation of <seealso cref="IWordRepository"/>.</param>
    /// <param name="dictionaryRepository">Implementation of <seealso cref="IDictionaryRepository"/>.</param>
    /// <param name="contentTypeRepository">Implementation of <seealso cref="IContentTypeRepository"/>.</param>
    /// <param name="languageRepository">Implementation of <seealso cref="ILanguageRepository"/>.</param>
    /// <param name="configuration">Implementation of ASP.NET configuration API.</param>
    /// <param name="backgroundJobClient">Implementation of <seealso cref="IBackgroundJobClient"/>.</param>
    public DictionariesController(IWordRepository wordRepository, 
        IDictionaryRepository dictionaryRepository, 
        IContentTypeRepository contentTypeRepository,
        ILanguageRepository languageRepository, 
        IConfiguration configuration,
        IBackgroundJobClient backgroundJobClient)
    {
        _wordRepository = wordRepository;
        _dictionaryRepository = dictionaryRepository;
        _contentTypeRepository = contentTypeRepository;
        _languageRepository = languageRepository;
        _configuration = configuration;
        _backgroundJobClient = backgroundJobClient;
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
        IEnumerable<ContentType> allContentTypes =
            await _contentTypeRepository.GetContentTypesAsync(null, null, null, null, null);

        IndexViewModelFactory viewModelFactory = new IndexViewModelFactory(results.Results, currentPageNumber,
            numberOfPages, results.TotalUnpagedItems, maximumItems, sortBy, sortDirection, query,ietfLanguageTag, contentTypeId, allLanguages, allContentTypes);
        IndexViewModel viewModel = viewModelFactory.Create();
        return View(viewModel);
        
    }

    /// <summary>
    /// Dictionary Details view.
    /// </summary>
    /// <param name="id">Identifier of Dictionary.</param>
    /// <returns>View containing details of Dictionary.</returns>
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return BadRequest();
        
        Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(id.Value);
        if (dictionary == null) return NotFound();
    
        bool isPermittedToDelete = User.IsInRole("administrator");
        
        DetailsViewModelFactory viewModelFactory = new DetailsViewModelFactory(dictionary,isPermittedToDelete);
        DetailsViewModel viewModel = viewModelFactory.Create();
        return View(viewModel);
    }

    /// <summary>
    /// Page for confirmation of deleting a Dictionary.
    /// </summary>
    /// <param name="id">Identifier of Dictionary.</param>
    /// <returns>View containing confirmation fields.</returns>
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        
        Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(id.Value);
        if (dictionary == null) return NotFound();
        
        DeleteViewModelFactory viewModelFactory = new DeleteViewModelFactory(dictionary);
        DeleteViewModel viewModel = viewModelFactory.Create();
        return View(viewModel);
    }

    /// <summary>
    /// Submits a Dictionary deletion job for processing.
    /// </summary>
    /// <param name="viewModel">View Model for Dictionary deletion.</param>
    /// <returns>View including status of deletion submission.</returns>
    [HttpPost]
    public async Task<IActionResult> Delete(DeleteViewModel viewModel)
    {
        Dictionary? dictionary = await _dictionaryRepository.GetByIdAsync(viewModel.Id);
        if (dictionary == null) return NotFound();
        
        DeleteViewModelFactory viewModelFactory = new DeleteViewModelFactory(dictionary);
        viewModelFactory.Configure(ref viewModel);

        bool isPermittedToDelete = User.IsInRole("administrator");
        if (!isPermittedToDelete) ModelState.AddModelError("", "You are not permitted to delete this item.");
        
        if (viewModel.VerificationCode != viewModel.ConfirmVerificationCode) ModelState.AddModelError("ConfirmVerificationCode", "Verification code does not match");

        if (ModelState.IsValid)
        {
            // add deletion as job for Hangfire
            viewModel.DeleteJobSubmittedSuccessfully = true;
            
            _backgroundJobClient.Enqueue<DeleteDictionaryJob>(job =>
                job.DeleteDictionary(dictionary.Id)
            );
            
        }
        
        return View(viewModel);
    }
    
}