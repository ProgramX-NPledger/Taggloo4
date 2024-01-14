using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DictionaryRepository : IDictionaryRepository
{
	private readonly DataContext _dataContext;

	public DictionaryRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	public void Create(Dictionary dictionary)
	{
		_dataContext.Dictionaries.Add(dictionary);
	}
	
	public void Update(Dictionary dictionary)
	{
		_dataContext.Entry(dictionary).State = EntityState.Modified;
	}

	public async Task<bool> SaveAllAsync()
	{
		return await _dataContext.SaveChangesAsync() > 0;
	}

	public async Task<Dictionary?> GetById(int id)
	{
		return await _dataContext.Dictionaries.SingleOrDefaultAsync(q => q.Id == id);
	}


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