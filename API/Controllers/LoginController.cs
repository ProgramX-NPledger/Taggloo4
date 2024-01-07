using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.DTO;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Login methods.
/// </summary>
[Authorize]
public class LoginController : BaseApiController
{
	private readonly DataContext _dataContext;
	private readonly ITokenService _tokenService;

	public LoginController(DataContext dataContext, ITokenService tokenService)
	{
		_dataContext = dataContext;
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
	public async Task<ActionResult<LoggedInUser>> Post(LoginUser loginUser)
	{
		AppUser appUser =
			await _dataContext.Users.SingleOrDefaultAsync(q => q.UserName == loginUser.UserName.ToLower());
		if (appUser == null) return Unauthorized();

		using (HMACSHA512 hmacSha512 = new HMACSHA512(appUser.PasswordSalt))
		{
			byte[] hashedPasswordBytes = hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(loginUser.Password));

			if (hashedPasswordBytes.Length != appUser.PasswordHash.Length) return Unauthorized();
			for (int i = 0; i < hashedPasswordBytes.Length; i++)
			{
				if (hashedPasswordBytes[i] != appUser.PasswordHash[i]) return Unauthorized();
			}

		}

		return new LoggedInUser()
		{
			UserName = appUser.UserName,
			Token = _tokenService.CreateToken(appUser)
		};
	}
}