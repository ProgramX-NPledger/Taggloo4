﻿using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : IdentityDbContext<AppUser, 
	AppRole, 
	int, 
	IdentityUserClaim<int>,
	AppUserRole,
	IdentityUserLogin<int>,
	IdentityRoleClaim<int>,
	IdentityUserToken<int>>
{
	public DbSet<Language> Languages { get; set; }
	public DbSet<Dictionary> Dictionaries { get; set; }
	public DbSet<Word> Words { get; set; }
	
	public DataContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<AppUser>()
			.HasMany(ur => ur.UserRoles)
			.WithOne(u => u.User)
			.HasForeignKey(ur => ur.UserId)
			.IsRequired();

		builder.Entity<AppRole>()
			.HasMany(ur => ur.UserRoles)
			.WithOne(r => r.Role)
			.HasForeignKey(ur => ur.RoleId)
			.IsRequired();

		builder.Entity<Language>()
			.HasMany(l => l.Dictionaries)
			.WithOne(d => d.Language)
			.HasForeignKey(d => d.IetfLanguageTag)
			.IsRequired();

		builder.Entity<Dictionary>()
			.HasMany(d => d.Words)
			.WithOne(w => w.Dictionary)
			.HasForeignKey(w => w.DictionaryId)
			.IsRequired();

	}
}