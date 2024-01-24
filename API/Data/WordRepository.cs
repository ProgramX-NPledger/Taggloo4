using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class WordRepository : IWordRepository
{
	private readonly DataContext _dataContext;

	public WordRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	public void Create(Word word)
	{
		_dataContext.Words.Add(word);
	}
	
	public void Update(Word word)
	{
		_dataContext.Entry(word).State = EntityState.Modified;
	}

	public async Task<bool> SaveAllAsync()
	{
		return await _dataContext.SaveChangesAsync() > 0;
	}

	public async Task<IEnumerable<Word>> GetWords(string word, int? dictionaryId)
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

		return await query.ToArrayAsync();
	}

	public async Task<Word?> GetById(int id)
	{
		return await _dataContext.Words.SingleOrDefaultAsync(q => q.Id == id);
	}
}