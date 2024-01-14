using API.Model;

namespace API.Contract;

public interface IDictionaryRepository
{
	void Update(Dictionary dictionary);
	void Create(Dictionary dictionary);
	Task<bool> SaveAllAsync();
	//Task<IEnumerable<Word>> GetAllWordsAsync();
	// Task<AppUser> GetUserByIdAsync(int id);
	// Task<AppUser> GetUserByUserNameAsync(string userName);
	
}