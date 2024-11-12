using Taggloo4.Web.Areas.Admin.Models.DTOs;
using Taggloo4.Web.Model;
using Taggloo4.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract;
using Taggloo4.Model.Exceptions;
using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Home.Factory;

/// <summary>
/// Configures non-transient data for a View Model implementation of <seealso cref="IDictionariesSummaryViewModel"/>.
/// </summary>
public class DictionariesSummaryViewModelFactory : IPartialViewModelFactory<IDictionariesSummaryViewModel>
{
    private readonly int _numberOfLanguagesInDictionaries;
    private readonly int _numberOfDictionaries;
    private readonly int _numberOfContentTypes;
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DictionariesSummaryViewModelFactory(DictionariesSummary dictionariesSummary)
    {
        _numberOfDictionaries=dictionariesSummary.NumberOfDictionaries;
        _numberOfLanguagesInDictionaries = dictionariesSummary.NumberOfLanguagesInDictionaries;
        _numberOfContentTypes = dictionariesSummary.NumberOfContentTypes;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref IDictionariesSummaryViewModel viewModel)
    {
        viewModel.NumberOfLanguagesInDictionaries = _numberOfLanguagesInDictionaries;
        viewModel.NumberOfDictionaries = _numberOfDictionaries;
        viewModel.NumberOfContentTypes = _numberOfContentTypes;
    }

}