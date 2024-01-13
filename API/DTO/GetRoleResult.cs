using System.Text.Json.Serialization;

namespace API.DTO;

/// <summary>
/// Represents a Role
/// </summary>
public class GetRoleResult
{
	/// <summary>
	/// Name of the Role.
	/// </summary>
	public string RoleName { get; set; }

	/// <summary>
	/// A list of Links referring to related Entities.
	/// </summary>
	public IEnumerable<Link> Links { get; set; }
	
}