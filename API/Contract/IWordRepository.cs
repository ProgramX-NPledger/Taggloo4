using API.Model;

namespace API.Contract;

public interface IWordRepository
{
	void Update(Word word);
	void Create(Word word);
	Task<bool> SaveAllAsync();

	Task<Word?> GetWordByWordWithinDictionary(string word, int dictionaryId);

	Task<Word?> GetById(int id);
	
	//Task<IEnumerable<Word>> GetAllWordsAsync();
	// Task<AppUser> GetUserByIdAsync(int id);
	// Task<AppUser> GetUserByUserNameAsync(string userName);

}