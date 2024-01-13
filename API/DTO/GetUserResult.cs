using System.Text.Json.Serialization;

namespace API.DTO;

/// <summary>
/// Represents a User
/// </summary>
public class GetUserResult
{
	/// <summary>
	/// Username of the user, used to log in.
	/// </summary>
	public string UserName { get; set; }

	/// <summary>
	/// A list of Links referring to related Entities.
	/// </summary>
	public IEnumerable<Link> Links { get; set; }
	
	/// <summary>
	/// The roles the user is a member of
	/// </summary>
	public IEnumerable<string> HasRoles { get; set; }
}