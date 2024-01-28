using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Represents a repository for working with Translations.
/// </summary>
public class TranslationRepository : ITranslationRepository
{
	private readonly DataContext _dataContext;

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public TranslationRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="wordTranslation">Entity to mark as updated.</param>
	public void Update(WordTranslation wordTranslation)
	{
		_dataContext.Entry(wordTranslation).State = EntityState.Modified;
	}

	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="wordTranslation">Entity to create.</param>
	public void Create(WordTranslation wordTranslation)
	{
		_dataContext.WordTranslations.Add(wordTranslation);
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
	/// Retrieves a <seealso cref="WordTranslation"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="WordTranslation"/>.</param>
	/// <returns>The requested <seealso cref="WordTranslation"/>, or <c>null</c> if no Word Translation could be found./</returns>
	public async Task<WordTranslation?> GetById(int id)
	{
		// TODO: widen this out to other translation types
		return await _dataContext.WordTranslations.SingleOrDefaultAsync(q => q.Id == id);
	}

}