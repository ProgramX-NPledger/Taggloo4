using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Represents a repository for working with Languages.
/// </summary>
public class TranslatorRepository : ITranslatorRepository
{
	private readonly DataContext _dataContext;

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public TranslatorRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	/// <inheritdoc cref="ITranslationRepository"/>
	public async Task<IEnumerable<Translator>> GetAllTranslatorsAsync(string? key, bool? isEnabled)
	{
		IQueryable<Translator> query = _dataContext.Translators
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