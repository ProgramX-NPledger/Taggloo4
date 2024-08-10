using API.Contract;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.ViewModels.Home.Factory;

/// <summary>
/// Configures non-transient data for a View Model implementation of <seealso cref="ILargeTranslateFormViewModel"/>.
/// </summary>
public class LargeTranslateFormViewModelFactory : IPartialViewModelFactory<ILargeTranslateFormViewModel>
{

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref ILargeTranslateFormViewModel viewModel)
    {
        viewModel.AllLanguages = new[]
        {
            new SelectListItem("Text1", "Value1", true),
            new SelectListItem("Text2", "Value2", false)
        };
    }
}