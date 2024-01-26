using API.Model;
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
	public DbSet<WordTranslation> WordTranslations { get; set; }
	public DbSet<ApiLog> ApiLogs { get; set; }
	

	
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

		builder.Entity<Dictionary>()
			.HasMany(d => d.WordTranslations)
			.WithOne(wt => wt.Dictionary)
			.HasForeignKey(wt => wt.DictionaryId)
			.IsRequired();

		builder.Entity<Word>()
			.HasIndex(a =>
				new
				{
					a.DictionaryId,
					a.TheWord
				}).IsUnique();

		builder.Entity<Word>()
			.HasMany(w => w.Translations)
			.WithOne(wt => wt.FromWord)
			.HasForeignKey(wt => wt.FromWordId)
			.IsRequired();

		// builder.Entity<Word>()
		// 	.HasMany(w => w.Translations)
		// 	.WithOne(wt => wt.ToWord)
		// 	.HasForeignKey(wt => wt.ToWordId)
		// 	.IsRequired();
		
		// builder.Entity<WordTranslation>()
		// 	.HasOne(wt=>wt.Dictionary)
		// 	.WithOne(d=>d. // TODO: Understand Many to one relationship
		// 	
			
		

	}
}