using Microsoft.EntityFrameworkCore;
using Taggloo4.Model;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Data;

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
	/// <param name="externalId">An externally determined identifier.</param>
	/// <returns>A collection of matching <seealso cref="Word"/>s within the <seealso cref="Dictionary"/>.</returns>
	public async Task<IEnumerable<Word>> GetWordsAsync(string? word, int? dictionaryId, string? externalId)
	{
		IQueryable<Word> query = _dataContext.Words
			.Include("Translations")
			.Include("AppearsInPhrases")
			.Include("Dictionary")
			.AsQueryable();
		
		if (!string.IsNullOrWhiteSpace(word))
		{
			query = query.Where(q => q.TheWord == word);
		}

		if (dictionaryId.HasValue)
		{
			query = query.Where(q => q.DictionaryId == dictionaryId.Value);
		}

		if (!string.IsNullOrWhiteSpace(externalId))
		{
			query = query.Where(q => q.ExternalId == externalId);
		}

		return await query.ToArrayAsync();
	}

	/// <summary>
	/// Retrieves a <seealso cref="Word"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Word"/>.</param>
	/// <returns>The requested <seealso cref="Word"/>, or <c>null</c> if no Word could be found./</returns>
	public async Task<Word?> GetByIdAsync(int id)
	{
		return await _dataContext.Words
			.Include("Dictionary")
			.Include("Translations")
			.Include("AppearsInPhrases")
			.SingleOrDefaultAsync(q => q.Id == id);
	}

	/// <summary>
	/// Retrieves matching <see cref="WordInPhrase"/> items.
	/// </summary>
	/// <param name="wordId">The ID of the <see cref="Word"/>.</param>
	/// <param name="phraseId">The ID of the <see cref="Phrase"/>.</param>
	/// <param name="ordinal">The ordinal of the Word.</param>
	/// <returns>A collection of matching <see cref="WordInPhrase"/>s.</returns>
	public async Task<IEnumerable<WordInPhrase>> GetPhrasesForWordAsync(int wordId, int phraseId, int ordinal)
	{
		return await _dataContext.WordsInPhrases
			.Include("Word")
			.Include("InPhrase")
			.Where(q => q.WordId == wordId && q.InPhraseId==phraseId && q.Ordinal==ordinal)
			.ToArrayAsync();
	}

	/// <summary>
	/// Adds a <see cref="WordInPhrase"/>, representing the <see cref="Word"/> appearing in a <see cref="Phrase"/>.
	/// </summary>
	/// <param name="wordInPhrase">The created <see cref="WordInPhrase"/> item.</param>
	public void AddPhraseForWord(WordInPhrase wordInPhrase)
	{
		_dataContext.WordsInPhrases.Add(wordInPhrase);
	}
}