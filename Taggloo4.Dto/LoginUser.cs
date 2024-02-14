namespace Taggloo4.Dto;

/// <summary>
/// Request for a User's login using the POST Login method.
/// </summary>
public class LoginUser
{
	/// <summary>
	/// UserName of user. This is case-insensitive.
	/// </summary>
	public required string UserName { get; set; }
	
	/// <summary>
	/// Password of user.
	/// </summary>
	public required string Password { get; set; }
}