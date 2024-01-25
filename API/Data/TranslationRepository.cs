using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class TranslationRepository : ITranslationRepository
{
	private readonly DataContext _dataContext;

	public TranslationRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	public void Update(WordTranslation wordTranslation)
	{
		_dataContext.Entry(wordTranslation).State = EntityState.Modified;
	}

	public void Create(WordTranslation wordTranslation)
	{
		_dataContext.WordTranslations.Add(wordTranslation);
	}
	
	

	public async Task<bool> SaveAllAsync()
	{
		return await _dataContext.SaveChangesAsync() > 0;
	}

	public async Task<WordTranslation?> GetById(int id)
	{
		return await _dataContext.WordTranslations.SingleOrDefaultAsync(q => q.Id == id);
	}

	// public async Task<Dictionary?> GetById(int id)
	// {
	// 	return await _dataContext.Dictionaries.SingleOrDefaultAsync(q => q.Id == id);
	// }


	// public async Task<IEnumerable<Language>> GetAllLanguagesAsync()
	// {
	// 	return await _dataContext.Languages.ToListAsync();
	// }



	// public async Task<AppUser> GetUserByUserNameAsync(string userName)
	// {
	// 	string lowerUserName = userName.ToLower();
	// 	return await _dataContext.Users.SingleOrDefaultAsync(q => q.UserName == lowerUserName);
	// }
}