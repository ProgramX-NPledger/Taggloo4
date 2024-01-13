using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{

	public static async Task SeedUsers(UserManager<AppUser> userManager)
	{
		// TODO Remove these seeded users
		if (await userManager.Users.AnyAsync()) return;

		string password = "AReallyStrongPa$$w0rd123";
		await userManager.CreateAsync(new AppUser()
		{
			UserName = "guestUser"
		}, password);

		await userManager.CreateAsync(new AppUser()
		{
			UserName = "dataExporter"
		}, password);

		await userManager.CreateAsync(new AppUser()
		{
			UserName = "dataImporter"
		}, password);

		await userManager.CreateAsync(new AppUser()
		{
			UserName = "translator"
		}, password);

		await userManager.CreateAsync(new AppUser()
		{
			UserName = "administrator",
		}, password);
	}
}