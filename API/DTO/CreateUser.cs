using System.ComponentModel.DataAnnotations;

namespace API.DTO;

/// <summary>
/// Represents a User to be created using the POST Users method.
/// </summary>
public class CreateUser
{
	/// <summary>
	/// UserName of the user. Whilst the casing may be mixed, internally all usernames are lower-case.
	/// </summary>
	[Required]
	public required string UserName { get; set; }
	
	/// <summary>
	/// The Password of the user. This is required to comply with minimum strength requirements.
	/// </summary>
	[Required]
	public required string Password { get; set; }
}