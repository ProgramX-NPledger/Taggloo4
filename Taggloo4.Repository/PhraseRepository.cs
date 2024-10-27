using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Phrases.
/// </summary>
public class PhraseRepository : RepositoryBase<Phrase>, IPhraseRepository
{
	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public PhraseRepository(DataContext dataContext) : base(dataContext)
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
	/// Retrieves a all matching <seealso cref="Phrase"/>s within a <seealso cref="Dictionary"/>.
	/// </summary>
	/// <param name="phrase">Word to match within the <seealso cref="Dictionary"/>.</param>
	/// <param name="dictionaryId">The ID of the <seealso cref="Dictionary"/> to search.</param>
	/// <param name="containingText">Filters response for presence of text (collation as per database).</param>
	/// <param name="externalId">If specified, filters using an externally determined identifier.</param>
	/// <param name="languageCode">If specified, filters within Dictionaries for the IETF Language Tag.</param>
	/// <returns>A collection of matching <seealso cref="Phrase"/>s within the <seealso cref="Dictionary"/>.</returns>
	public async Task<IEnumerable<Phrase>> GetPhrasesAsync(string? phrase, int? dictionaryId, string? containingText, string? externalId, string? languageCode)
	{
		IQueryable<Phrase> query = DataContext.Phrases
			.Include("Translations")
			.Include(m=>m.Dictionaries).ThenInclude(m=>m.Language)
			.AsQueryable();
		
		if (!string.IsNullOrWhiteSpace(phrase))
		{
			query = query.Where(q => q.ThePhrase == phrase);
		}

		if (dictionaryId.HasValue)
		{
			query = query.Where(q => q.DictionaryId == dictionaryId.Value);
		}

		if (!string.IsNullOrWhiteSpace(containingText))
		{
			query = query.Where(q => q.ThePhrase.Contains(containingText));
		}
		
		if (!string.IsNullOrWhiteSpace(externalId))
		{
			query = query.Where(q => q.ExternalId == externalId);
		}
		
		// the many:many to dictionaries is too complex for the EFCore LINQ->Sql transpilation so execute and perform
		// filter client-side
		List<Phrase> results = await query.ToListAsync();

		if (dictionaryId.HasValue)
		{
			results=results.Where(q =>q.Dictionaries.Select(qq=>qq.Id).Contains(dictionaryId.Value)).ToList();
		}
		
		if (!string.IsNullOrWhiteSpace(languageCode))
		{
			results = results.Where(q=>q.Dictionaries.Any(qq => qq.IetfLanguageTag == languageCode)).ToList();
		}
	
		return results;
		
	}

	/// <summary>
	/// Retrieves a <seealso cref="Phrase"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Phrase"/>.</param>
	/// <returns>The requested <seealso cref="Phrase"/>, or <c>null</c> if no Word could be found./</returns>
	public async Task<Phrase?> GetByIdAsync(int id)
	{
		return await DataContext.Phrases.SingleOrDefaultAsync(q => q.Id == id);
	}
	

	
}