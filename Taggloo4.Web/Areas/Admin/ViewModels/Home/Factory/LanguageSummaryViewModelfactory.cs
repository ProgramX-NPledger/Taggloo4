using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Model;
using Taggloo4.Web.Areas.Admin.Models.DTOs;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;
using Taggloo4.Web.ViewModels.Home;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Home.Factory;

/// <summary>
/// Configures non-transient data for a View Model implementation of <seealso cref="ILargeTranslateFormViewModel"/>.
/// </summary>
public class LanguageSummaryViewModelFactory : IPartialViewModelFactory<ILanguageSummaryViewModel>
{
    private readonly IEnumerable<Language> _allLanguages;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="allLanguages">All configured Languages, for populating Language selection fields.</param>
    public LanguageSummaryViewModelFactory(IEnumerable<Language> allLanguages)
    {
        _allLanguages = allLanguages;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref ILanguageSummaryViewModel viewModel)
    {
        viewModel.AllLanguages = _allLanguages.Select(q=>new LanguageDto()
        {
            Name = q.IetfLanguageTag,
            IetfLanguageTag = q.IetfLanguageTag,
            DictionariesCount = (q.Dictionaries?.Count()) ?? 0
        });
    }

}