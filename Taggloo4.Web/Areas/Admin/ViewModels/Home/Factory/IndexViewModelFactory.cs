using Taggloo4.Contract;
using Taggloo4.Model;
using Taggloo4.Model.Exceptions;
using Taggloo4.Web.ViewModels.Home;
using Taggloo4.Web.ViewModels.Home.Factory;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Home.Factory;

/// <summary>
/// Creates and/or Configures a <seealso cref="Model.Home.IndexViewModel"/>.
/// </summary>
public class IndexViewModelFactory : IViewModelFactory<IndexViewModel>
{
    private readonly LanguageSummaryViewModelFactory _languageSummaryViewModelFactory;
    private readonly HangfireSummaryViewModelFactory _hangfireSummaryViewModelFactory;
    private readonly WordSummaryViewModelFactory _wordSummaryViewModelFactory;
    private readonly DictionariesSummaryViewModelFactory _dictionariesSummaryViewModelFactory;
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    public IndexViewModelFactory(IEnumerable<Language> allLanguages,
        int numberOfRecurringHangfireJobs, DateTime? lastHangfireJobExecution, DateTime? nextHangfireJobExecution,
        WordsSummary wordsSummary,
        DictionariesSummary dictionariesSummary)
    {
        _languageSummaryViewModelFactory = new LanguageSummaryViewModelFactory(allLanguages);
        _hangfireSummaryViewModelFactory = new HangfireSummaryViewModelFactory(numberOfRecurringHangfireJobs,
            lastHangfireJobExecution, nextHangfireJobExecution);
        _wordSummaryViewModelFactory = new WordSummaryViewModelFactory(wordsSummary);
        _dictionariesSummaryViewModelFactory = new DictionariesSummaryViewModelFactory(dictionariesSummary);
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
        ILanguageSummaryViewModel iLanguageSummaryViewModel = (ILanguageSummaryViewModel)viewModel;
        _languageSummaryViewModelFactory.Configure(ref iLanguageSummaryViewModel);
        
        IHangfireSummaryViewModel iHangfireSummaryViewModel = (IHangfireSummaryViewModel)viewModel;
        _hangfireSummaryViewModelFactory.Configure(ref iHangfireSummaryViewModel);
        
        IWordSummaryViewModel iWordSummaryViewModel = (IWordSummaryViewModel)viewModel;
        _wordSummaryViewModelFactory.Configure(ref iWordSummaryViewModel);
        
        IDictionariesSummaryViewModel iDictionariesSummaryViewModel = (IDictionariesSummaryViewModel)viewModel;
        _dictionariesSummaryViewModelFactory.Configure(ref iDictionariesSummaryViewModel);
        
    }
}