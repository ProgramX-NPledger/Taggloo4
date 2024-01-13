using System.Text.Json.Serialization;

namespace API.DTO;

/// <summary>
/// Result of a user creation request.
/// </summary>
public class CreateUserResult
{
	/// <summary>
	/// The username chosen for the user.
	/// </summary>
	[JsonPropertyName("userName")]
	public string UserName { get; set; }

	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link> Links { get; set; }
	
	/// <summary>
	/// A list of Roles that the user is allocated.
	/// </summary>
	//public IEnumerable<string> Roles { get; set; }
	// TODO return Roles added to new users by default
	
}