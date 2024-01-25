using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LanguageRepository : ILanguageRepository
{
	private readonly DataContext _dataContext;

	public LanguageRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	public void Create(Language language)
	{
		_dataContext.Languages.Add(language);
	}
	
	public void Update(Language language)
	{
		_dataContext.Entry(language).State = EntityState.Modified;
	}

	public async Task<bool> SaveAllAsync()
	{
		return await _dataContext.SaveChangesAsync() > 0;
	}

	public async Task<IEnumerable<Language>> GetAllLanguagesAsync()
	{
		return await _dataContext.Languages.ToListAsync();
	}

	public async Task<Language?> GetLanguageByIetfLanguageTag(string ietfLanguageTag)
	{
		return  await _dataContext.Languages.SingleOrDefaultAsync(q => q.IetfLanguageTag == ietfLanguageTag);
	}


	// public async Task<AppUser> GetUserByUserNameAsync(string userName)
	// {
	// 	string lowerUserName = userName.ToLower();
	// 	return await _dataContext.Users.SingleOrDefaultAsync(q => q.UserName == lowerUserName);
	// }
}