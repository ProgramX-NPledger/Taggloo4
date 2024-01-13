using API.Model;

namespace API.Contract;

public interface IUserRepository
{
	void Update(AppUser user);
	Task<bool> SaveAllAsync();
	Task<IEnumerable<AppUser>> GetUsersAsync();
	Task<AppUser> GetUserByIdAsync(int id);
	Task<AppUser> GetUserByUserNameAsync(string userName);
	
}