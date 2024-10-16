using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Taggloo4.Contract;
using Taggloo4.Contract.Criteria;
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


	/// <inheritdoc cref="GetWordsAsync"/>
	public async Task<IEnumerable<Word>> GetWordsAsync(string? word, int? dictionaryId, string? externalId, string? ietfLanguageTag)
	{
		IQueryable<Word> query = DataContext.Words
			.Include("Translations")
			.Include("Dictionaries.Language")
			.Include("AppearsInPhrases")
			.Include(m=>m.Dictionaries)
			.AsQueryable();
		
		if (!string.IsNullOrWhiteSpace(word))
		{
			query = query.Where(q => q.TheWord == word);
		}

		if (dictionaryId.HasValue)
		{
			query = query.Where(q => q.Dictionaries!=null && q.Dictionaries.Select(qq=>qq.Id).Contains(dictionaryId.Value));
		}

		if (!string.IsNullOrWhiteSpace(externalId))
		{
			query = query.Where(q => q.ExternalId == externalId);
		}

		if (!string.IsNullOrWhiteSpace(ietfLanguageTag))
		{
			query = query.Where(q=>q.Dictionaries!=null && q.Dictionaries.Any(qq => qq.IetfLanguageTag == ietfLanguageTag));
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
			.Include("Dictionaries.Language")
			.Include("ToTranslations.FromWord")
			.Include("FromTranslations.ToWord")
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

	/// <inheritdoc cref="IWordRepository.GetWordsSummaryAsync"/>
	public async Task<WordsSummary> GetWordsSummaryAsync()
	{
		WordsInDictionariesSummary[] wordsInDictionariesSummaries=await DataContext.WordsInDictionariesSummaries.ToArrayAsync();
		WordsSummary wordsSummary = new WordsSummary()
		{
			TotalWords = wordsInDictionariesSummaries.Sum(q=>q.WordCount ?? 0),
			AcrossDictionariesCount = wordsInDictionariesSummaries.Length,
			LastWordCreatedTimeStamp = wordsInDictionariesSummaries.Max(q=>q.LatestWordCreatedAt)
		};
		return wordsSummary;
	}

	/// <inheritdoc cref="IWordRepository.GetWordsByCriteriaAsync"/>
	public async Task<PagedResults<WordInDictionary>> GetWordsByCriteriaAsync(GetWordsCriteria criteria)
	{
		var query = DataContext.WordsInDictionaries.AsQueryable();
		
		if (criteria.DictionaryId.HasValue) query = query.Where(q => q.DictionaryId == criteria.DictionaryId.Value);
		
		if (!string.IsNullOrWhiteSpace(criteria.Query)) query = query.Where(q=>(q.TheWord ?? string.Empty).Contains(criteria.Query));

		if (!string.IsNullOrWhiteSpace(criteria.IetfLanguageTag)) query = query.Where(q => q.IetfLanguageTag==criteria.IetfLanguageTag);
		
		if (criteria.DictionaryId.HasValue) query = query.Where(q => q.DictionaryId == criteria.DictionaryId.Value);
		
		PagedResults<WordInDictionary> results = new PagedResults<WordInDictionary>()
		{
			TotalUnpagedItems = await query.CountAsync()
		};
		
		switch (criteria.SortBy)
		{
			case WordsSortColumn.TheWord:
				if (criteria.SortDirection==SortDirection.Ascending) query = query.OrderBy(q => q.TheWord);
				else query = query.OrderByDescending(q => q.TheWord);
				break;
			case WordsSortColumn.WordId:
				if (criteria.SortDirection == SortDirection.Ascending) query = query.OrderBy(q => q.WordId);
				else query = query.OrderByDescending(q => q.WordId);
				break;
			case WordsSortColumn.Dictionary:
				if (criteria.SortDirection == SortDirection.Ascending) query=query.OrderBy(q=>q.DictionaryName);
				else query = query.OrderByDescending(q=>q.DictionaryName);
				break;
			case WordsSortColumn.Language:
				if (criteria.SortDirection == SortDirection.Ascending) query=query.OrderBy(q=>q.LanguageName);
				else query = query.OrderByDescending(q=>q.LanguageName);
				break;
			case WordsSortColumn.AppearsInPhrases:
				if (criteria.SortDirection == SortDirection.Ascending) query=query.OrderBy(q=>q.AppearsInPhrasesCount);
				else query = query.OrderByDescending(q=>q.AppearsInPhrasesCount);
				break;
		}

		results.Results = await query
			.Skip(criteria.OrdinalOfFirstItem)
			.Take(criteria.ItemsPerPage)
			.ToArrayAsync();
		
		return results;
	}
}