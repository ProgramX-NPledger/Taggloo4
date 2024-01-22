using API.Model;

namespace API.Contract;

public interface ITranslationRepository
{
	void Update(WordTranslation wordTranslation);
	void Create(WordTranslation wordTranslation); // overload this for different translation types
	Task<bool> SaveAllAsync();

	// Task<WordTranslation?> GetWordByWordWithinDictionary(string word, int dictionaryId);
	//
	Task<WordTranslation?> GetById(int id);
	
	//Task<IEnumerable<Word>> GetAllWordsAsync();
	// Task<AppUser> GetUserByIdAsync(int id);
	// Task<AppUser> GetUserByUserNameAsync(string userName);

}