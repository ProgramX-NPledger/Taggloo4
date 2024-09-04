using Taggloo4.Web.Model;

namespace Taggloo4.Web.Contract;

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