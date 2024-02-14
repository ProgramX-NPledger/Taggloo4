using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Entity Framework Data Context.
/// </summary>
public class DataContext : IdentityDbContext<AppUser, 
	AppRole, 
	int, 
	IdentityUserClaim<int>,
	AppUserRole,
	IdentityUserLogin<int>,
	IdentityRoleClaim<int>,
	IdentityUserToken<int>>
{
	/// <summary>
	/// Languages.
	/// </summary>
	public DbSet<Language> Languages { get; set; }
	
	/// <summary>
	/// Dictionaries
	/// </summary>
	public DbSet<Dictionary> Dictionaries { get; set; }
	
	/// <summary>
	/// Words
	/// </summary>
	public DbSet<Word> Words { get; set; }
	
	/// <summary>
	/// Word Translations
	/// </summary>
	public DbSet<WordTranslation> WordTranslations { get; set; }
	
	/// <summary>
	/// API Logs
	/// </summary>
	public DbSet<ApiLog> ApiLogs { get; set; }
	

	/// <summary>
	/// Constructor with options parameter.
	/// </summary>
	/// <param name="options">Configure the connection to the database.</param>
	public DataContext(DbContextOptions options) : base(options)
	{
	}

	/// <summary>
	/// Helper function for creation of migrations and called to allow configuration of database.
	/// </summary>
	/// <param name="builder">Entity Framework <seealso cref="ModelBuilder"/>.</param>
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

		// this results in migration failure
		// builder.Entity<Word>()
		// 	.HasMany(w => w.Translations)
		// 	.WithOne(wt => wt.FromWord)
		// 	.HasForeignKey(wt => wt.FromWordId)
		// 	.OnDelete(DeleteBehavior.NoAction);
		//
		// builder.Entity<Word>()
		// 	.HasMany(w => w.Translations)
		// 	.WithOne(wt => wt.ToWord)
		// 	.HasForeignKey(wt => wt.ToWordId)
		// 	.OnDelete(DeleteBehavior.NoAction);
			

	}
}