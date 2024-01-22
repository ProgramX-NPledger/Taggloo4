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

	public async Task<Word?> GetWordByWordWithinDictionary(string word, int dictionaryId)
	{
		return await _dataContext.Words.SingleOrDefaultAsync(q => q.TheWord == word && q.DictionaryId==dictionaryId);
	}

	public async Task<Word?> GetById(int id)
	{
		return await _dataContext.Words.SingleOrDefaultAsync(q => q.Id == id);
	}
}