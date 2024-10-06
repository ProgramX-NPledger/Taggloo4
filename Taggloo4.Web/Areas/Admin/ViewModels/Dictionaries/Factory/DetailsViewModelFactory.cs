using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries.Factory;

/// <summary>
/// Factory for constructing a configured <seealso cref="DetailsViewModel"/>.
/// </summary>
public class DetailsViewModelFactory : IViewModelFactory<DetailsViewModel>
{
    private readonly Dictionary _dictionary;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public DetailsViewModelFactory(Dictionary dictionary)
    {
        _dictionary = dictionary;
    }

    /// <inheritdoc cref="IViewModelFactory{TViewModelType}"/>
    public DetailsViewModel Create()
    {
        DetailsViewModel viewModel = new DetailsViewModel()
        {
            Id = _dictionary.Id,
            Name = _dictionary.Name,
            Description = _dictionary.Description,
            CreatedOn = _dictionary.CreatedOn,
            LanguageName = _dictionary.Language?.Name ?? "(no name)",
            SourceUrl = _dictionary.SourceUrl,
            IetfLanguageTag = _dictionary.IetfLanguageTag,
            CreatedByUserName = _dictionary.CreatedByUserName,
            CreatedAt = _dictionary.CreatedAt,
        };
        Configure(ref viewModel);

        return viewModel;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref DetailsViewModel viewModel)
    {
        viewModel.Id = _dictionary.Id;
        viewModel.Name = _dictionary.Name;
        viewModel.Description = _dictionary.Description;
        viewModel.CreatedOn = _dictionary.CreatedOn;
        viewModel.LanguageName = _dictionary.Language?.Name ?? "(no name)";
        viewModel.SourceUrl = _dictionary.SourceUrl;
        viewModel.IetfLanguageTag = _dictionary.IetfLanguageTag;
        viewModel.CreatedByUserName = _dictionary.CreatedByUserName;
        viewModel.CreatedAt = _dictionary.CreatedAt;
        viewModel.ContentTypeId = _dictionary.ContentTypeId;
        viewModel.ContentTypeKey = _dictionary.ContentType?.ContentTypeKey ?? "(no key)";
        viewModel.ContentTypeController = _dictionary.ContentType?.Controller ?? "(no controller)";
        viewModel.ContentTypeNameSingular = _dictionary.ContentType?.NameSingular ?? "(no name)";
        viewModel.ContentTypeNamePlural = _dictionary.ContentType?.NamePlural ?? "(no name)";
    }
}
