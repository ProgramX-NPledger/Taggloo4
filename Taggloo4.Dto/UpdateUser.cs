using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a User to be updated using the PATCH Users method.
/// </summary>
public class UpdateUser
{

	/// <summary>
	/// Username of the User to update.
	/// </summary>
	public required string UserName { get; set; }

	/// <summary>
	/// Role names the User should be a Member of.
	/// </summary>
	public string[]? Roles { get; set; }

	/// <summary>
	/// If set, changes the User's password.
	/// </summary>
	public ChangePassword? ChangePassword { get; set; }
	
	
	
}