using Microsoft.AspNetCore.Identity;

namespace API.Model;

/// <summary>
/// Role within the application. Inherits from ASP.NET Identity implementation of <seealso cref="IdentityRole"/>.
/// </summary>
public class AppRole : IdentityRole<int>
{
	/// <summary>
	/// Collection of <seealso cref="AppUserRole"/> allowing navigation to Users in this Role.
	/// </summary>
	public ICollection<AppUserRole>? UserRoles { get; set; }
	
}