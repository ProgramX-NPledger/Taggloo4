using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.ViewModels.Home.Factory;

/// <summary>
/// Configures non-transient data for a View Model implementation of <seealso cref="ILargeTranslateFormViewModel"/>.
/// </summary>
public class LargeTranslateFormViewModelFactory : IPartialViewModelFactory<ILargeTranslateFormViewModel>
{
    private readonly IEnumerable<Language> _allLanguages;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="allLanguages">All configured Languages, for populating Language selection fields.</param>
    public LargeTranslateFormViewModelFactory(IEnumerable<Language> allLanguages)
    {
        _allLanguages = allLanguages;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref ILargeTranslateFormViewModel viewModel)
    {
        viewModel.AllLanguages = _allLanguages.Select(q=>new SelectListItem()
        {
            Text = q.Name,
            Value = q.IetfLanguageTag,
        });
    }

    /// <summary>
    /// Sets the two Language selections to be opposing, thereby providing a 50% chance of getting the
    /// user story correct. Client-side script is used to flip the selection should this guess be wrong.
    /// </summary>
    /// <param name="viewModel">The View Model being prepared.</param>
    /// <remarks>
    /// This should only be called at initial creation of the model, to avoid over-writing the user's
    /// selection.
    /// </remarks>
    public void TransposeLanguageSelections(ref ILargeTranslateFormViewModel viewModel)
    {
        if (viewModel.AllLanguages != null)
        {
            viewModel.FromLanguageCode = viewModel.AllLanguages.First().Value;
            viewModel.ToLanguageCode = viewModel.AllLanguages.Last().Value;
        }
    }
}