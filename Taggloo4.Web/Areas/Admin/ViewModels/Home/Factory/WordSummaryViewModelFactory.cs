using Taggloo4.Web.Areas.Admin.Models.DTOs;
using Taggloo4.Web.Model;
using Taggloo4.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract;
using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Home.Factory;

/// <summary>
/// Configures non-transient data for a View Model implementation of <seealso cref="IWordSummaryViewModel"/>.
/// </summary>
public class WordSummaryViewModelFactory : IPartialViewModelFactory<IWordSummaryViewModel>
{
    private readonly int _totalWords;
    private readonly int _acrossDictionariesCount;
    private readonly DateTime? _lastWordCreatedTimeStamp;
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    public WordSummaryViewModelFactory(WordsSummary wordsSummary)
    {
        _totalWords = wordsSummary.TotalWords;
        _acrossDictionariesCount = wordsSummary.AcrossDictionariesCount;
        _lastWordCreatedTimeStamp = wordsSummary.LastWordCreatedTimeStamp;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref IWordSummaryViewModel viewModel)
    {
        viewModel.TotalWords = _totalWords;
        viewModel.AcrossDictionariesCount = _acrossDictionariesCount;
        viewModel.LastWordCreatedTimeStamp = _lastWordCreatedTimeStamp;
    }

}