using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Helper;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class UsersController : BaseApiController
{
	private readonly DataContext _dataContext;

	public UsersController(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	/// <summary>
	/// Gets all Users.
	/// </summary>
	/// <returns></returns>
	/// <response code="200">Request was successful.</response>
	[HttpGet]
	// TODO: Add parameters to allow filtering, paging, return 400 if bad request
	public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
	{
		List<AppUser> users = await _dataContext.Users.ToListAsync();
		return users;
	}

	/// <summary>
	/// Retrieve user details.
	/// </summary>
	/// <param name="id">User ID of user.</param>
	/// <returns>A user</returns>
	/// <response code="200">User is found.</response>
	/// <response code="404">User is not found.</response>
	[HttpGet("{id}")]
	public async Task<ActionResult<AppUser?>> GetUser(int id)
	{
		AppUser? user = await _dataContext.Users.FindAsync(id);
		if (user == null) return NotFound();
		return user;
	}

	/// <summary>
	/// Creates a new User.
	/// </summary>
	/// <param name="createUser">A <see cref="CreateUser"/> representing the User to create.</param>
	/// <returns>The created User.</returns>
	/// <response code="200">User was created.</response> // TODO: Should be 201
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	[HttpPost]
	public async Task<ActionResult<AppUser>> CreateUser(CreateUser createUser)
	{
		if (!PasswordStrength.IsPasswordStrongEnough(createUser.Password)) return BadRequest("Password does not meet minimum strength requirements");
		
		if (await IsUserExisting(createUser.UserName)) return BadRequest("UserName already in use");
		
		using (HMACSHA512 hmacSha512 = new HMACSHA512())
		{
			AppUser newUser = new AppUser()
			{
				UserName = createUser.UserName.ToLower(), // all usernames are lowered for comparison
				PasswordHash = hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(createUser.Password)),
				PasswordSalt = hmacSha512.Key
			};
			_dataContext.Users?.Add(newUser);
			await _dataContext.SaveChangesAsync();
			return newUser;
			// TODO: Return correct HTTP response
		}
	}

	private async Task<bool> IsUserExisting(string userName)
	{
		string lowerUserName = userName.ToLower();
		return _dataContext.Users != null && await _dataContext.Users.AnyAsync(q => q.UserName==lowerUserName);
	}
}