﻿using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with authentication tokens.
/// </summary>
public interface ITokenService
{
	/// <summary>
	/// Creates a token for use in authenticating the User.
	/// </summary>
	/// <param name="user">The user requiring authentication.</param>
	/// <returns>The token representing the authentication.</returns>
	Task<string> CreateToken(AppUser user);
}