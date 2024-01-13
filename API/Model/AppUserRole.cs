using Microsoft.AspNetCore.Identity;

namespace API.Model;

public class AppUserRole : IdentityUserRole<int>
{
	public AppUser? User { get; set; }
	public AppRole? Role { get; set; }

}