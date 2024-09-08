using Microsoft.AspNetCore.Identity;

namespace Taggloo4.Data.EntityFrameworkCore.Identity;

/// <summary>
/// User within the application. Inherits from ASP.NET Identity implementation of <seealso cref="IdentityUser"/>.
/// </summary>
public class AppUser : IdentityUser<int>
{
	/// <summary>
	/// Collection of <seealso cref="AppUserRole"/> allowing to navigation of Roles for this User.
	/// </summary>
	public ICollection<AppUserRole>? UserRoles { get; set; }	
}