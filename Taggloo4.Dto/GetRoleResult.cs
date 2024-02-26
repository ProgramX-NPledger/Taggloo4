using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Role
/// </summary>
public class GetRoleResult
{
	/// <summary>
	/// Name of the Role.
	/// </summary>
	public required string RoleName { get; set; }

	/// <summary>
	/// A list of Links referring to related Entities.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
	
}