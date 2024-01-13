﻿using System.Security.Cryptography;
using System.Text;
using API.Contract;
using API.Data;
using API.DTO;
using API.Helper;
using API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseApiController
{
	private readonly IUserRepository _userRepository;

	public UsersController(IUserRepository userRepository)
	{
		_userRepository = userRepository;
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
		return Ok(await _userRepository.GetUsersAsync());
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
		AppUser? user = await _userRepository.GetUserByUserNameAsync(userName);
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
		
		// using (HMACSHA512 hmacSha512 = new HMACSHA512())
		// {
			AppUser newUser = new AppUser()
			{
				UserName = createUser.UserName.ToLower(), // all usernames are lowered for comparison
				// PasswordHash = hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(createUser.Password)),
				// PasswordSalt = hmacSha512.Key
			};
			_userRepository.Update(newUser);
			await _userRepository.SaveAllAsync();

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
		// }
	}

	private async Task<bool> IsUserExisting(string userName)
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		return await _userRepository.GetUserByUserNameAsync(userName)!=null;
	}
}