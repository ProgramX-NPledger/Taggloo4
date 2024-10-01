using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Words.Factory;

/// <summary>
/// Factory for constructing a configured <seealso cref="DetailsViewModel"/>.
/// </summary>
public class DetailsViewModelFactory : IViewModelFactory<DetailsViewModel>
{
    private readonly Word _word;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public DetailsViewModelFactory(Word word)
    {
        _word = word;
    }

    /// <inheritdoc cref="IViewModelFactory{TViewModelType}"/>
    public DetailsViewModel Create()
    {
        DetailsViewModel viewModel = new DetailsViewModel()
        {
            Id = _word.Id,
            TheWord = _word.TheWord,
            CreatedByUserName = _word.CreatedByUserName,
            CreatedOn = _word.CreatedOn,
            DictionaryName = _word.Dictionary?.Name ?? "(no name)",
            LanguageName = _word.Dictionary?.Language?.Name ?? "(no name)",
            IetfLanguageTag = _word.Dictionary?.IetfLanguageTag ?? "(no tag)",
        };
        Configure(ref viewModel);

        return viewModel;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref DetailsViewModel viewModel)
    {
        viewModel.Id = _word.Id;
        viewModel.CreatedAt = _word.CreatedAt;
        viewModel.CreatedOn = _word.CreatedOn;
        viewModel.CreatedByUserName = _word.CreatedByUserName;
        viewModel.ExternalId = _word.ExternalId ?? "(none)";
        viewModel.TheWord = _word.TheWord;
        viewModel.DictionaryId = _word.Dictionary?.Id ?? -1;
        viewModel.DictionaryName = _word.Dictionary?.Name ?? "(no name)";
        viewModel.LanguageName = _word.Dictionary?.Language?.Name ?? "(no name)";
        viewModel.IetfLanguageTag = _word.Dictionary?.IetfLanguageTag ?? "(no tag)";

    }
}
