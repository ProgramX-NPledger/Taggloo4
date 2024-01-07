using API.Model;

namespace API.Contract;

public interface ITokenService
{
	/// <summary>
	/// Creates a token for use in authenticating the User.
	/// </summary>
	/// <param name="user">The user requiring authentication.</param>
	/// <returns>The token representing the authentication.</returns>
	string CreateToken(AppUser user);
}