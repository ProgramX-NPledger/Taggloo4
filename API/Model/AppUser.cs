using Microsoft.AspNetCore.Identity;

namespace API.Model;

public class AppUser : IdentityUser<int>
{
	public ICollection<AppUserRole> UserRoles { get; set; }	
}