using API.Contract;
using API.Model;
using API.Model.Home;

namespace API.ViewModels.Home.Factory;

/// <summary>
/// Creates and/or Configures a <seealso cref="IndexViewModel"/>.
/// </summary>
public class IndexViewModelFactory : IViewModelFactory<IndexViewModel>
{
    private readonly LargeTranslateFormViewModelFactory _largeTranslateFormViewModelFactory;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public IndexViewModelFactory(IEnumerable<Language> allLanguages)
    {
        _largeTranslateFormViewModelFactory = new LargeTranslateFormViewModelFactory(allLanguages);
    }
    
    /// <inheritdoc cref="IViewModelFactory{TViewModelType}"/>
    public IndexViewModel Create()
    {
        IndexViewModel viewModel = new IndexViewModel();
        Configure(ref viewModel);
        
        ILargeTranslateFormViewModel iLargeTranslateFormViewModel = (ILargeTranslateFormViewModel)viewModel;
        _largeTranslateFormViewModelFactory.TransposeLanguageSelections(ref iLargeTranslateFormViewModel);
        return viewModel;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref IndexViewModel viewModel)
    {
        ILargeTranslateFormViewModel iLargeTranslateFormViewModel = (ILargeTranslateFormViewModel)viewModel;
        _largeTranslateFormViewModelFactory.Configure(ref iLargeTranslateFormViewModel);
    }
}