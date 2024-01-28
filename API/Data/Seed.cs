﻿using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Services for seeding of database.
/// </summary>
public class Seed
{

	/// <summary>
	/// Seeds Users into the database.
	/// </summary>
	/// <param name="userManager">Implementation of <seealso cref="UserManager&lt;AppUser&gt;"/></param>
	/// <param name="roleManager">Implementation of <seealso cref="RoleManager&lt;AppRole&gt;"/></param>
	public static async Task SeedUsers(UserManager<AppUser> userManager,
		RoleManager<AppRole> roleManager)
	{
		// TODO Remove these seeded users
		if (await userManager.Users.AnyAsync()) return;

		var roles = new List<AppRole>
		{
			new AppRole()
			{
				Name = "dataImporter"
			},
			new AppRole()
			{
				Name = "dataExporter"
			},
			new AppRole()
			{
				Name = "translator"
			},
			new AppRole()
			{
				Name = "administrator"
			}
		};

		foreach (AppRole role in roles)
		{
			await roleManager.CreateAsync(role);
		}
		
		string password = "AReallyStrongPa$$w0rd123";
		AppUser guestUser = new AppUser()
		{
			UserName = "guestUser"
		};
		await userManager.CreateAsync(guestUser,password);

		string[] defaultUsers = new string[]
		{
			"dataExporter",
			"dataImporter",
			"translator",
			"administrator"
		};
		foreach (string defaultUser in defaultUsers)
		{
			AppUser dataExporterUser = new AppUser()
			{
				UserName = defaultUser
			};
			await userManager.CreateAsync(dataExporterUser,password);
			await userManager.AddToRoleAsync(dataExporterUser,defaultUser);
		}
	}
}