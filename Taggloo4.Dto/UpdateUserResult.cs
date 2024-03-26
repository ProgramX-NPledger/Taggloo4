
using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Result of a User update request.
/// </summary>
public class UpdateUserResult
{

	/// <summary>
	/// The UserName of the User.
	/// </summary>
	public required string UserName { get; set; }

	/// <summary>
	/// List of Roles the User has been added to.
	/// </summary>
	public required string[] AddedToRoles { get; set; }
	
	/// <summary>
	/// List of Roles the User has been removed from.
	/// </summary>
	public required string[] RemovedFromRoles { get; set; }

	public bool PasswordChanged { get; set; }
	
	/// <summary>
	/// List of related Entities
	/// </summary>
	[JsonPropertyName("links")]
	public IEnumerable<Link>? Links { get; set; }
	
}