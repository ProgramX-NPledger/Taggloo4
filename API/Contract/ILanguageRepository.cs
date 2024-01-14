using API.Model;

namespace API.Contract;

public interface ILanguageRepository
{
	void Update(Language user);
	void Create(Language language);
	Task<bool> SaveAllAsync();
	Task<IEnumerable<Language>> GetAllLanguagesAsync();
	// Task<AppUser> GetUserByIdAsync(int id);
	// Task<AppUser> GetUserByUserNameAsync(string userName);
	
}