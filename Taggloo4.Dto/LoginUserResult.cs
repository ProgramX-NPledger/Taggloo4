﻿namespace Taggloo4.Dto;

/// <summary>
/// Represents a successful log in for a User, including a JWT Token that must be
/// provided for all authenticated requests.
/// </summary>
public class LoginUserResult
{
	/// <summary>
	/// UserName of the User.
	/// </summary>
	public string UserName { get; set; }
	
	/// <summary>
	/// JWT Token representing the secure log-in.
	/// </summary>
	public string Token { get; set; }
}