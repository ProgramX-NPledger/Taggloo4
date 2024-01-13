using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
	private readonly DataContext _dataContext;

	public UserRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}
	
	public void Update(AppUser user)
	{
		_dataContext.Entry(user).State = EntityState.Modified;
	}

	public async Task<bool> SaveAllAsync()
	{
		return await _dataContext.SaveChangesAsync() > 0;
	}

	public async Task<IEnumerable<AppUser>> GetUsersAsync()
	{
		return await _dataContext.Users.ToListAsync();
	}

	public async Task<AppUser> GetUserByIdAsync(int id)
	{
		return await _dataContext.Users.FindAsync(id);
	}

	public async Task<AppUser> GetUserByUserNameAsync(string userName)
	{
		string lowerUserName = userName.ToLower();
		return await _dataContext.Users.SingleOrDefaultAsync(q => q.UserName == lowerUserName);
	}
}