using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Languages.
/// </summary>
public class TranslatorConfigurationRepository : RepositoryBase<TranslatorConfiguration>, ITranslatorConfigurationRepository
{
	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public TranslatorConfigurationRepository(DataContext dataContext) : base(dataContext)
	{
	}

	/// <inheritdoc cref="ITranslatorConfigurationRepository.GetTranslatorConfiguration"/>
	public async Task<TranslatorConfiguration?> GetTranslatorConfiguration(string translatorKey)
	{
		return await DataContext.TranslatorConfigurations.AsNoTracking()
			.SingleOrDefaultAsync(q => q.Key == translatorKey);
	}
}