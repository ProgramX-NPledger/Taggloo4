﻿using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Words.
/// </summary>
public class WordRepository : RepositoryBase<Word>, IWordRepository
{
	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public WordRepository(DataContext dataContext) : base(dataContext)
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
	/// Retrieves all matching <seealso cref="Word"/>s within a <seealso cref="Dictionary"/>.
	/// </summary>
	/// <param name="word">Word to match within the <seealso cref="Dictionary"/>.</param>
	/// <param name="dictionaryId">The ID of the <seealso cref="Dictionary"/> to search.</param>
	/// <param name="externalId">An externally determined identifier.</param>
	/// <returns>A collection of matching <seealso cref="Word"/>s within the <seealso cref="Dictionary"/>.</returns>
	public async Task<IEnumerable<Word>> GetWordsAsync(string? word, int? dictionaryId, string? externalId)
	{
		IQueryable<Word> query = DataContext.Words
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
		return await DataContext.Words
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
		return await DataContext.WordsInPhrases
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
		DataContext.WordsInPhrases.Add(wordInPhrase);
	}
}