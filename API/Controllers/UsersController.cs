using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.DTO;
using API.Helper;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseApiController
{
	private readonly UserManager<AppUser> _userManager;

	public UsersController(UserManager<AppUser> userManager)
	{
		_userManager = userManager;
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
		return Ok(await _userManager.Users.ToListAsync());
//		return users; // TODO use RESTful DTO
	}

	

	/// <summary>
	/// Retrieve user details.
	/// </summary>
	/// <param name="userName">User Name of user.</param>
	/// <returns>A user.</returns>
	/// <response code="200">User is found.</response>
	/// <response code="403">Not permitted.</response>
	/// <response code="404">User is not found.</response>
	[HttpGet("{userName}")]
	public async Task<ActionResult<GetUserResult>> GetUser(string userName)
	{
		string upperedUserName = userName.ToUpper();
		AppUser? user = await _userManager.Users.SingleOrDefaultAsync(q => q.NormalizedUserName == upperedUserName);
		if (user == null) return NotFound();
		
		return new GetUserResult()
		{
			UserName = user.UserName,
			Links = new []
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new string[] { JSON_MIME_TYPE },
					HRef = $"{GetBaseApiPath()}/users/{user.UserName}" 
				}
			}
		};

		throw new NullReferenceException("_dataContext.Users");
	}
	
    /// <summary>
	/// Creates a new User.
	/// </summary>
	/// <param name="createUser">A <see cref="CreateUser"/> representing the User to create.</param>
	/// <returns>The created User.</returns>
	/// <response code="201">User was created.</response>
	/// <response code="400">One or more validation errors prevented successful creation.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPost]
	public async Task<ActionResult<AppUser>> CreateUser(CreateUser createUser)
	{
		if (!PasswordStrength.IsPasswordStrongEnough(createUser.Password)) return BadRequest("Password does not meet minimum strength requirements");
		
		if (await IsUserExisting(createUser.UserName)) return BadRequest("UserName already in use");
		
		AppUser newUser = new AppUser()
		{
			UserName = createUser.UserName.ToLower(), // all usernames are lowered for comparison
		};

		var result = await _userManager.CreateAsync(newUser, createUser.Password);
		if (!result.Succeeded) return BadRequest(result.Errors);

		string url = $"{GetBaseApiPath()}/users/{newUser.UserName}";
		CreateUserResult createUserResult = new CreateUserResult()
		{
			UserName = newUser.UserName,
			Links = new []
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					Types = new []{ JSON_MIME_TYPE },
					HRef = url
				}
			}
		};
		return Created(url, createUserResult);
	}

	private async Task<bool> IsUserExisting(string userName)
	{
		string loweredUserName = userName.ToLower();
		return await _userManager.Users.AnyAsync(q => q.UserName == loweredUserName);
	}
}