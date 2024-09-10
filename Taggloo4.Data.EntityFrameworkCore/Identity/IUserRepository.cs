namespace Taggloo4.Data.EntityFrameworkCore.Identity;

/// <summary>
/// Represents an abstraction for working with Users.
/// </summary>
public interface IUserRepository
{
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="user">Entity to mark as updated.</param>
	void Update(AppUser user);

	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();
	
	/// <summary>
	/// Retrieves all Users.
	/// </summary>
	/// <returns>A collection of <seealso cref="AppUser"/>s.</returns>
	Task<IEnumerable<AppUser>> GetUsersAsync();
	
	/// <summary>
	/// Retrieves an <seealso cref="AppUser"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="AppUser"/>.</param>
	/// <returns>The requested <seealso cref="AppUser"/>, or <c>null</c> if no User could be found./</returns>
	Task<AppUser?> GetUserByIdAsync(int id);
	
	/// <summary>
	/// Retrieves an <seealso cref="AppUser"/> by its UserName.
	/// </summary>
	/// <param name="userName">The userName of the <seealso cref="AppUser"/>.</param>
	/// <returns>The requested <seealso cref="AppUser"/>, or <c>null</c> if no User could be found./</returns>
	Task<AppUser?> GetUserByUserNameAsync(string userName);
	
}