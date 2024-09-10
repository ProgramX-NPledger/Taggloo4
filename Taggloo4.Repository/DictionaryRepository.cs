using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Dictionaries.
/// </summary>
public class DictionaryRepository : RepositoryBase<Dictionary>, IDictionaryRepository
{
	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public DictionaryRepository(DataContext dataContext) : base(dataContext)
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
	/// Retrieves a <seealso cref="Dictionary"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Dictionary"/>.</param>
	/// <returns>The requested <seealso cref="Dictionary"/>, or <c>null</c> if no Dictionary could be found./</returns>
	public async Task<Dictionary?> GetByIdAsync(int id)
	{
		return await DataContext.Dictionaries.SingleOrDefaultAsync(q => q.Id == id);
	}

	/// <summary>
	/// Retrieves all matching <seealso cref="Dictionary"/> items.
	/// </summary>
	/// <param name="id">If specified, limits by the ID of the Dictionary.</param>
	/// <param name="ietfLanguageTag">If specified, limits by the IETF Language Tag for the Dictionary.</param>
	/// <returns>A collection of matching <seealso cref="Dictionary"/> items.</returns>
	public async Task<IEnumerable<Dictionary>> GetDictionariesAsync(int? id, string? ietfLanguageTag)
	{
		IQueryable<Dictionary> query = DataContext.Dictionaries.AsQueryable();
		if (id.HasValue)
		{
			query = query.Where(q => q.Id == id.Value);
		}

		if (!string.IsNullOrWhiteSpace(ietfLanguageTag))
		{
			query = query.Where(q => q.IetfLanguageTag == ietfLanguageTag);
		}

		return await query.ToArrayAsync();
	}

	/// <summary>
	/// Deletes the specified Dictionary, and all related content.
	/// </summary>
	/// <param name="dictionaryId">ID of Dictionary to delete.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Thrown if deletion fails.</exception>
	public async Task Delete(int dictionaryId)
	{
		Dictionary dictionary = DataContext.Dictionaries.Single(q => q.Id == dictionaryId);
		DataContext.Dictionaries.Remove(dictionary);
		await DataContext.SaveChangesAsync();
	}
}