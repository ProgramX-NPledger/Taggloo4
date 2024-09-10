using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Languages.
/// </summary>
public class LanguageRepository : RepositoryBase<Language>, ILanguageRepository
{
	

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public LanguageRepository(DataContext dataContext) : base(dataContext)
	{
		
	}
	

	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	public async Task<bool> SaveAllAsync()
	{
		return await DataContext.SaveChangesAsync() > 0;
	}

	/// <summary>
	/// Retrieves all Languages.
	/// </summary>
	/// <returns>A collection of Languages.</returns>
	public async Task<IEnumerable<Language>> GetAllLanguagesAsync()
	{
		return await DataContext.Languages
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
		return  await DataContext.Languages.SingleOrDefaultAsync(q => q.IetfLanguageTag == ietfLanguageTag);
	}

	/// <summary>
	/// Retrieves all matching <seealso cref="Language"/>s.
	/// </summary>
	/// <param name="ietfLanguageTag">If specified, the IETF Language Tag of the <seealso cref="Language"/>.</param>
	/// <returns>A collection of matching <seealso cref="Language"/>s.</returns>
	public async Task<IEnumerable<Language>> GetLanguagesAsync(string? ietfLanguageTag)
	{
		IQueryable<Language> query = DataContext.Languages.AsQueryable();
		if (!string.IsNullOrWhiteSpace(ietfLanguageTag))
		{
			query = query.Where(q => q.IetfLanguageTag == ietfLanguageTag);
		}

		return await query.ToArrayAsync();
	}
}