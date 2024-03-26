using System.Security.Cryptography;
using System.Text;
using System.Transactions;
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

    
	/// <summary>
	/// Update an existing User.
	/// </summary>
	/// <param name="updateUser">A <see cref="UpdateUser"/> representing the User to update.</param>
	/// <returns>The updated User.</returns>
	/// <response code="200">User was updated.</response>
	/// <response code="400">One or more validation errors prevented successful updating.</response>
	/// <response code="403">Not permitted.</response>
	[HttpPatch]
	[Authorize(Roles="administrator")]
	public async Task<ActionResult<AppUser>> UpdateUser(UpdateUser updateUser)
	{
		AppUser? user=await _userManager.Users.SingleOrDefaultAsync(q => q.UserName == updateUser.UserName);
		if (user==null) return NotFound();

		UpdateUserResult updateUserResult = new UpdateUserResult()
		{
			UserName = user.UserName!,
			AddedToRoles = Array.Empty<string>(),
			RemovedFromRoles = Array.Empty<string>()
			
		};
			
		// role changes
		IEnumerable<string>? removeFromRoles = null;
		IEnumerable<string>? addToRoles = null;
		if (updateUser.Roles != null)
		{
			IList<string> userRoles = await _userManager.GetRolesAsync(user);
			removeFromRoles = GetRolesToRemoveUserFrom(user, userRoles, updateUser.Roles).ToArray();
			addToRoles = GetRolesToAddUserTo(user, userRoles, updateUser.Roles);
			IEnumerable<string> rolesToBeRemovedAndAdded = removeFromRoles.Where(q => addToRoles.Contains(q));
			if (rolesToBeRemovedAndAdded.Any())
			{
				return BadRequest("It is not permitted to Remove and Add the same Role");
			}
		}
		
		// change password
		if (updateUser.ChangePassword != null)
		{
			if (string.IsNullOrWhiteSpace(updateUser.ChangePassword.CurrentPassword))
				return BadRequest("CurrentPassword is required");
			if (string.IsNullOrWhiteSpace(updateUser.ChangePassword.NewPassword))
				return BadRequest("NewPassword is required");
		}
		
		// we do the actual operations in a transaction to make this HTTP call idempotent
		using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
		{
			List<string> removedFromRoles = new List<string>();
			if (removeFromRoles != null && removeFromRoles.Any())
			{
				foreach (string removeFromRole in removeFromRoles)
				{
					IdentityResult removeFromRoleIdentityResult =
						await _userManager.RemoveFromRoleAsync(user, removeFromRole);
					if (!removeFromRoleIdentityResult.Succeeded)
					{
						return BadRequest(
							$"Failed to remove User from Role '{removeFromRole}' due to {string.Join(", ", removeFromRoleIdentityResult.Errors.Select(q => $"{q.Code} {q.Description}").ToArray())}");
					}
					removedFromRoles.Add(removeFromRole);
				}
			}
			updateUserResult.RemovedFromRoles = removedFromRoles.ToArray();

			List<string> addedToRoles = new List<string>();
			if (addToRoles!=null && addToRoles.Any())
			{
				foreach (string addToRole in addToRoles)
				{
					IdentityResult addToRoleIdentityResult =  await _userManager.AddToRoleAsync(user, addToRole);
					if (!addToRoleIdentityResult.Succeeded)
					{
						return BadRequest(
							$"Failed to add User to Role '{addToRole}' due to {string.Join(", ", addToRoleIdentityResult.Errors.Select(q => $"{q.Code}: {q.Description}").ToArray())}");
					}
					addedToRoles.Add(addToRole);
				}
			}
			updateUserResult.AddedToRoles = addedToRoles.ToArray();

			if (updateUser.ChangePassword != null)
			{
				IdentityResult changePasswordIdentityResult = await _userManager.ChangePasswordAsync(user,
					updateUser.ChangePassword.CurrentPassword, updateUser.ChangePassword.NewPassword);
				if (!changePasswordIdentityResult.Succeeded)
				{
					return BadRequest(
						$"Failed to change password due to {string.Join(", ", changePasswordIdentityResult.Errors.Select(q => $"{q.Code}: {q.Description}"))}");
				}
				updateUserResult.PasswordChanged = true;
			}

			transactionScope.Complete();
		}
		
		string url = $"{GetBaseApiPath()}/users/{user.UserName}";
		updateUserResult.Links = new[]
		{
			new Link()
			{
				Action = "get",
				Rel = "self",
				Types = new string[] { JSON_MIME_TYPE },
				HRef = url
			}
		};
		
		return Ok(updateUserResult);
		
		
	}

	private IEnumerable<string> GetRolesToAddUserTo(AppUser user, IList<string> userRoles, string[] updateUserRoles)
	{
		return updateUserRoles.Where(q => !userRoles.Contains(q));
	}

	private IEnumerable<string> GetRolesToRemoveUserFrom(AppUser user, IList<string> userRoles, string[] updateUserRoles)
	{
		return userRoles.Where(q => !updateUserRoles.Contains(q));
	}
	

	private async Task<bool> IsUserExisting(string userName)
	{
		string loweredUserName = userName.ToLower();
		return await _userManager.Users.AnyAsync(q => q.UserName == loweredUserName);
	}
}