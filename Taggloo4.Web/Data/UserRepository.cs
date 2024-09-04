using Microsoft.EntityFrameworkCore;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Data;

/// <summary>
/// Represents a repository for working with Users.
/// </summary>
public class UserRepository : IUserRepository
{
	private readonly DataContext _dataContext;

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public UserRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}
	
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="user">Entity to mark as updated.</param>
	public void Update(AppUser user)
	{
		_dataContext.Entry(user).State = EntityState.Modified;
	}

	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	public async Task<bool> SaveAllAsync()
	{
		return await _dataContext.SaveChangesAsync() > 0;
	}

	/// <summary>
	/// Retrieves all Users.
	/// </summary>
	/// <returns>A collection of <seealso cref="AppUser"/>s.</returns>
	public async Task<IEnumerable<AppUser>> GetUsersAsync()
	{
		return await _dataContext.Users.ToListAsync();
	}

	/// <summary>
	/// Retrieves an <seealso cref="AppUser"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="AppUser"/>.</param>
	/// <returns>The requested <seealso cref="AppUser"/>, or <c>null</c> if no User could be found./</returns>
	public async Task<AppUser?> GetUserByIdAsync(int id)
	{
		return await _dataContext.Users.FindAsync(id);
	}

	/// <summary>
	/// Retrieves an <seealso cref="AppUser"/> by its UserName.
	/// </summary>
	/// <param name="userName">The userName of the <seealso cref="AppUser"/>.</param>
	/// <returns>The requested <seealso cref="AppUser"/>, or <c>null</c> if no User could be found./</returns>
	public async Task<AppUser?> GetUserByUserNameAsync(string userName)
	{
		string lowerUserName = userName.ToLower();
		return await _dataContext.Users.SingleOrDefaultAsync(q => q.UserName == lowerUserName);
	}
}