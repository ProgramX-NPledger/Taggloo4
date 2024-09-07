using Microsoft.EntityFrameworkCore;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Data;

/// <summary>
/// Represents a repository for working with Languages.
/// </summary>
public class TranslatorConfigurationRepository : ITranslatorConfigurationRepository
{
	private readonly DataContext _dataContext;

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public TranslatorConfigurationRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	/// <inheritdoc cref="ITranslatorConfigurationRepository.GetTranslatorConfiguration"/>
	public async Task<TranslatorConfiguration?> GetTranslatorConfiguration(string translatorKey)
	{
		return await _dataContext.TranslatorConfigurations.AsNoTracking()
			.SingleOrDefaultAsync(q => q.Key == translatorKey);
	}
}