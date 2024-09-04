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

	/// <inheritdoc cref="ITranslationRepository"/>
	public async Task<IEnumerable<TranslatorConfiguration>> GetAllTranslatorsAsync(string? key, bool? isEnabled)
	{
		IQueryable<TranslatorConfiguration> query = _dataContext.TranslatorConfigurations
			.AsNoTracking()
			.AsQueryable();
		
		if (!string.IsNullOrWhiteSpace(key))
		{
			query = query.Where(q => q.Key == key);
		}
		
		if (isEnabled.HasValue)
		{
			if (isEnabled.Value)
				query = query.Where(q => q.IsEnabled);
			else
				query = query.Where(q => !q.IsEnabled);
		}

		return await query.ToArrayAsync();
	}
}