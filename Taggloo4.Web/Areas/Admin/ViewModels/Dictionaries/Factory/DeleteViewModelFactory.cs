using Microsoft.AspNetCore.Mvc.Rendering;
using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Dictionaries.Factory;

/// <summary>
/// Factory for constructing a configured <seealso cref="DetailsViewModel"/>.
/// </summary>
public class DeleteViewModelFactory : IViewModelFactory<DeleteViewModel>
{
    private readonly Dictionary _dictionary;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public DeleteViewModelFactory(Dictionary dictionary)
    {
        _dictionary = dictionary;
    }

    /// <inheritdoc cref="IViewModelFactory{TViewModelType}"/>
    public DeleteViewModel Create()
    {
        DeleteViewModel viewModel = new DeleteViewModel()
        {
            Id = _dictionary.Id,
            Name = _dictionary.Name,
            VerificationCode = Guid.NewGuid().ToString().Substring(0, 7),
            LanguageName = _dictionary.Language?.Name ?? "(no name)",
            ContentTypeNamePlural = _dictionary.ContentType?.NamePlural,
            HasValidConfiguredDictionaryManager = !string.IsNullOrEmpty(_dictionary.DictionaryManagerDotNetAssemblyName) && !string.IsNullOrWhiteSpace(_dictionary.DictionaryManagerDotNetTypeName)
        };
        Configure(ref viewModel);

        return viewModel;
    }

    /// <inheritdoc cref="IPartialViewModelFactory{TViewModelType}"/>
    public void Configure(ref DeleteViewModel viewModel)
    {
        viewModel.Id = _dictionary.Id;
        viewModel.Name = _dictionary.Name;
        viewModel.LanguageName = _dictionary.Language?.Name ?? "(no name)";
        viewModel.ContentTypeNamePlural = _dictionary.ContentType?.NamePlural ?? "(no name)";
        viewModel.HasValidConfiguredDictionaryManager =
            !string.IsNullOrEmpty(_dictionary.DictionaryManagerDotNetAssemblyName) &&
            !string.IsNullOrWhiteSpace(_dictionary.DictionaryManagerDotNetTypeName);

    }
}
