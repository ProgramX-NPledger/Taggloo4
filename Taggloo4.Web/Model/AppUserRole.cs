using Microsoft.AspNetCore.Identity;

namespace Taggloo4.Web.Model;

/// <summary>
/// Membership of a User within a Role. Inherits from ASP.NET Identity implementation of <seealso cref="AppUserRole"/>.
/// </summary>
public class AppUserRole : IdentityUserRole<int>
{
	/// <summary>
	/// The User associated with the Role membership.
	/// </summary>
	public AppUser? User { get; set; }
	
	/// <summary>
	/// The Role of the membership.
	/// </summary>
	public AppRole? Role { get; set; }

}