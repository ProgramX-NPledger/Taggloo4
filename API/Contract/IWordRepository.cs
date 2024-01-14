using API.Model;

namespace API.Contract;

public interface IWordRepository
{
	// TODO Word1
	//void Update(Word word);
	//void Create(Word word);
	Task<bool> SaveAllAsync();
	//Task<IEnumerable<Word>> GetAllWordsAsync();
	// Task<AppUser> GetUserByIdAsync(int id);
	// Task<AppUser> GetUserByUserNameAsync(string userName);
	
}