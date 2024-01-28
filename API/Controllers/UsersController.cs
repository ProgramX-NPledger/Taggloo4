using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.Helper;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseApiController
{
	private readonly UserManager<AppUser> _userManager;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="userManager">Implementation of <seealso cref="UserManager{AppUser}"/>.</param>
	public UsersController(UserManager<AppUser> userManager)
	{
		_userManager = userManager;
	}
	

	/// <summary>
	/// Retrieve user details.
	/// </summary>
	/// <param name="userName">User Name of user.</param>
	/// <returns>A user.</returns>
	/// <response code="200">User is found.</response>
	/// <response code="403">Not permitted.</response>
	/// <response code="404">User is not found.</response>
	[Authorize(Roles="administrator")]
	[HttpGet("{userName}")]
	public async Task<ActionResult<GetUserResult>> GetUser(string userName)
	{
		string upperedUserName = userName.ToUpper();
		AppUser? user = await _userManager.Users.SingleOrDefaultAsync(q => q.NormalizedUserName == upperedUserName);
		if (user == null) return NotFound();

		List<Link> links = new List<Link>
		{
			new Link()
			{
				Action = "get",
				Rel = "self",
				Types = new string[] { JSON_MIME_TYPE },
				HRef = $"{GetBaseApiPath()}/users/{user.UserName}" 
			}
		};

		IList<string> roles = await _userManager.GetRolesAsync(user);
		roles.ToList().ForEach(x =>
		{
			links.Add(new Link()
			{
				Action = "get",
				Rel = "role",
				Types = new string[] { JSON_MIME_TYPE },
				HRef = $"{GetBaseApiPath()}/roles/{x}"
			});
		});
		
		return new GetUserResult()
		{
			UserName = user.UserName ?? string.Empty,
			HasRoles = await _userManager.GetRolesAsync(user),
			Links = links
		};
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
	[Authorize(Roles="administrator")]
	public async Task<ActionResult<AppUser>> CreateUser(CreateUser createUser)
	{
		if (!PasswordStrength.IsPasswordStrongEnough(createUser.Password)) return BadRequest("Password does not meet minimum strength requirements");
		
		if (await IsUserExisting(createUser.UserName)) return BadRequest("UserName already in use");
		
		AppUser newUser = new AppUser()
		{
			UserName = createUser.UserName
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