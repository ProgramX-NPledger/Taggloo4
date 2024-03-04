using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Represents a repository for working with Phrases.
/// </summary>
public class PhraseRepository : IPhraseRepository
{
	private readonly DataContext _dataContext;

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public PhraseRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="phrase">Entity to create.</param>
	public void Create(Phrase phrase)
	{
		_dataContext.Phrases.Add(phrase);
	}
	
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="phrase">Entity to mark as updated.</param>
	public void Update(Phrase phrase)
	{
		_dataContext.Entry(phrase).State = EntityState.Modified;
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
	/// Retrieves all matching <seealso cref="Phrase"/>s within a <seealso cref="Dictionary"/>.
	/// </summary>
	/// <param name="phrase">Phrase to match within the <seealso cref="Dictionary"/>.</param>
	/// <param name="dictionaryId">The ID of the <seealso cref="Dictionary"/> to search.</param>
	/// <returns>A collection of matching <seealso cref="Phrase"/>s within the <seealso cref="Dictionary"/>.</returns>
	public async Task<IEnumerable<Phrase>> GetPhrasesAsync(string? phrase, int? dictionaryId)
	{
		IQueryable<Phrase> query = _dataContext.Phrases.AsQueryable();
		if (!string.IsNullOrWhiteSpace(phrase))
		{
			query = query.Where(q => q.ThePhrase == phrase);
		}

		if (dictionaryId.HasValue)
		{
			query = query.Where(q => q.DictionaryId == dictionaryId.Value);
		}

		return await query.ToArrayAsync();
	}

	/// <summary>
	/// Retrieves a <seealso cref="Phrase"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Phrase"/>.</param>
	/// <returns>The requested <seealso cref="Phrase"/>, or <c>null</c> if no Word could be found./</returns>
	public async Task<Phrase?> GetByIdAsync(int id)
	{
		return await _dataContext.Phrases.SingleOrDefaultAsync(q => q.Id == id);
	}
}