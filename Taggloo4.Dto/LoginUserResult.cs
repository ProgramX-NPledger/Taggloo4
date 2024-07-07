namespace Taggloo4.Dto;

/// <summary>
/// Represents a successful log in for a User, including a JWT Token that must be
/// provided for all authenticated requests.
/// </summary>
public class LoginUserResult
{
	/// <summary>
	/// UserName of the User.
	/// </summary>
	public required string UserName { get; set; }
	
	/// <summary>
	/// JWT Token representing the secure log-in.
	/// </summary>
	public required string Token { get; set; }

	/// <summary>
	/// Email address of user.
	/// </summary>
	public string? Email { get; set; }

	/// <summary>
	/// SHA-256 hashed form of email address (for use by Gravatar).
	/// </summary>
	public string? EmailSha256 { get; set; }
	
}