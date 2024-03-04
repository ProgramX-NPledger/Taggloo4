using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Represents a repository for working with Words.
/// </summary>
public class WordRepository : IWordRepository
{
	private readonly DataContext _dataContext;

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public WordRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="word">Entity to create.</param>
	public void Create(Word word)
	{
		_dataContext.Words.Add(word);
	}
	
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="word">Entity to mark as updated.</param>
	public void Update(Word word)
	{
		_dataContext.Entry(word).State = EntityState.Modified;
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
	/// Retrieves all matching <seealso cref="Word"/>s within a <seealso cref="Dictionary"/>.
	/// </summary>
	/// <param name="word">Word to match within the <seealso cref="Dictionary"/>.</param>
	/// <param name="dictionaryId">The ID of the <seealso cref="Dictionary"/> to search.</param>
	/// <returns>A collection of matching <seealso cref="Word"/>s within the <seealso cref="Dictionary"/>.</returns>
	public async Task<IEnumerable<Word>> GetWordsAsync(string? word, int? dictionaryId)
	{
		IQueryable<Word> query = _dataContext.Words.AsQueryable();
		if (!string.IsNullOrWhiteSpace(word))
		{
			query = query.Where(q => q.TheWord == word);
		}

		if (dictionaryId.HasValue)
		{
			query = query.Where(q => q.DictionaryId == dictionaryId.Value);
		}

		return await query.Include("Dictionary").ToArrayAsync();
	}

	/// <summary>
	/// Retrieves a <seealso cref="Word"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Word"/>.</param>
	/// <returns>The requested <seealso cref="Word"/>, or <c>null</c> if no Word could be found./</returns>
	public async Task<Word?> GetById(int id)
	{
		return await _dataContext.Words.SingleOrDefaultAsync(q => q.Id == id);
	}
}