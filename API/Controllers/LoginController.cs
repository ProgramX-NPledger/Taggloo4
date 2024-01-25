using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Dto;

namespace API.Controllers;

/// <summary>
/// Login methods.
/// </summary>
[Authorize]
public class LoginController : BaseApiController
{
	private readonly UserManager<AppUser> _userManager;
	private readonly ITokenService _tokenService;

	public LoginController(UserManager<AppUser> userManager, ITokenService tokenService)
	{
		_userManager = userManager;
		_tokenService = tokenService;
	}

	/// <summary>
	/// Login a user by verifying their credentials and generating a secure JWT token for purpose
	/// of maintaining access over a period of time.
	/// </summary>
	/// <remarks>This method does not require authentication.</remarks>
	/// <param name="loginUser">The <see cref="LoginUser"/> representing the user logging in</param>
	/// <returns>A JWT token for purpose of maintaining access over a period of time.</returns>
	/// <response code="200">User successfully logged in.</response>
	/// <response code="401">User is unauthorised either because of invalid username or password.</response>
	[AllowAnonymous]
	[HttpPost]
	public async Task<ActionResult<LoginUserResult>> Post(LoginUser loginUser)
	{
		string upperedUserName = loginUser.UserName.ToUpper();
		AppUser? appUser = await _userManager.Users.SingleOrDefaultAsync(q => q.NormalizedUserName == upperedUserName);
		if (appUser == null) return Unauthorized();

		if (appUser.UserName == null) throw new NullReferenceException("appUser.UserName");
		
		bool isPasswordValid = await _userManager.CheckPasswordAsync(appUser, loginUser.Password);
		if (!isPasswordValid) return Unauthorized();

		return new LoginUserResult()
		{
			UserName = appUser.UserName,
			Token = await _tokenService.CreateToken(appUser)
		};
	}
}