using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Represents a repository for working with Languages.
/// </summary>
public class LanguageRepository : ILanguageRepository
{
	private readonly DataContext _dataContext;

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public LanguageRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="language">Entity to create.</param>
	public void Create(Language language)
	{
		_dataContext.Languages.Add(language);
	}
	
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="language">Entity to mark as updated.</param>
	public void Update(Language language)
	{
		_dataContext.Entry(language).State = EntityState.Modified;
	}

	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	public async Task<bool> SaveAllAsync()
	{
		return await _dataContext.SaveChangesAsync() > 0;
	}

	/// <summary>
	/// Retrieves all Languages.
	/// </summary>
	/// <returns>A collection of Languages.</returns>
	public async Task<IEnumerable<Language>> GetAllLanguagesAsync()
	{
		return await _dataContext.Languages
			.Include(q=>q.Dictionaries)
			.AsNoTracking()
			.ToListAsync();
	}

	/// <summary>
	/// Retrieves a Language by its IETF Language Tag.
	/// </summary>
	/// <param name="ietfLanguageTag">The IETF Language Tag of the Dictionary.</param>
	/// <returns>The requested Language, or <c>null</c> if no Language could be found./</returns>
	public async Task<Language?> GetLanguageByIetfLanguageTagAsync(string ietfLanguageTag)
	{
		return  await _dataContext.Languages.SingleOrDefaultAsync(q => q.IetfLanguageTag == ietfLanguageTag);
	}

	/// <summary>
	/// Retrieves a all matching <seealso cref="Language"/>s.
	/// </summary>
	/// <param name="ietfLanguageTag">If specified, the IETF Language Tag of the <seealso cref="Language"/>.</param>
	/// <returns>A collection of matching <seealso cref="Language"/>s.</returns>
	public async Task<IEnumerable<Language>> GetLanguagesAsync(string? ietfLanguageTag)
	{
		IQueryable<Language> query = _dataContext.Languages.AsQueryable();
		if (!string.IsNullOrWhiteSpace(ietfLanguageTag))
		{
			query = query.Where(q => q.IetfLanguageTag == ietfLanguageTag);
		}

		return await query.ToArrayAsync();
	}
}