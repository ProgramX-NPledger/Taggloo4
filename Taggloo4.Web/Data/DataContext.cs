using Taggloo4.Web.Translation.Translators;
using Taggloo4.Web.Translation.Translators.Factories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Data;

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
	/// Phrases
	/// </summary>
	public DbSet<Phrase> Phrases { get; set; }
	
	/// <summary>
	/// Word Translations
	/// </summary>
	public DbSet<WordTranslation> WordTranslations { get; set; }
	
	/// <summary>
	/// Phrase Translations
	/// </summary>
	public DbSet<PhraseTranslation> PhraseTranslations { get; set; }

	/// <summary>
	/// Words in Phrases.
	/// </summary>
	public DbSet<WordInPhrase> WordsInPhrases { get; set; }

	/// <summary>
	/// Reindexing Jobs
	/// </summary>
	public DbSet<ReindexingJob> ReindexingJobs { get; set; }

	/// <summary>
	/// Configuration data for Translators.
	/// </summary>
	public DbSet<TranslatorConfiguration> TranslatorConfigurations { get; set; }

	
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

		// words
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

		// phrases
		builder.Entity<Dictionary>()
			.HasMany(d => d.Phrases)
			.WithOne(w => w.Dictionary)
			.HasForeignKey(w => w.DictionaryId)
			.IsRequired();

		builder.Entity<Phrase>()
			.HasIndex(a =>
				new
				{
					a.DictionaryId,
					a.ThePhrase
				}).IsUnique();
		
		builder.Entity<Dictionary>()
			.HasMany(d => d.PhraseTranslations)
			.WithOne(pt => pt.Dictionary)
			.HasForeignKey(wt => wt.DictionaryId)
			.IsRequired();

		// builder.Entity<Phrase>()
		// 	.HasMany(p => p.Words)
		// 	.WithMany(w => w.Phrases)
		// 	.UsingEntity("WordsInPhrase");
		
		// translations
		
		builder.Entity<Word>()
			.HasMany(w => w.FromTranslations)
			.WithOne(wt => wt.FromWord)
			.HasForeignKey(wt => wt.FromWordId)
			.OnDelete(DeleteBehavior.NoAction);
		
		builder.Entity<Word>()
			.HasMany(w => w.ToTranslations)
			.WithOne(wt => wt.ToWord)
			.HasForeignKey(wt => wt.ToWordId)
			.OnDelete(DeleteBehavior.NoAction);
		
		builder.Entity<Phrase>()
			.HasMany(p => p.FromTranslations)
			.WithOne(pt => pt.FromPhrase)
			.HasForeignKey(pt => pt.FromPhraseId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.Entity<Phrase>()
			.HasMany(p => p.Translations)
			.WithOne(pt => pt.ToPhrase)
			.HasForeignKey(pt => pt.ToPhraseId)
			.OnDelete(DeleteBehavior.NoAction);
		
		// words in phrase

		builder.Entity<Phrase>()
			.HasMany(p => p.HasWordsInPhrase)
			.WithOne(wip => wip.InPhrase)
			.HasForeignKey(wip => wip.InPhraseId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.Entity<Word>()
			.HasMany(w => w.AppearsInPhrases)
			.WithOne(wip => wip.Word)
			.HasForeignKey(wip => wip.WordId)
			.OnDelete(DeleteBehavior.NoAction);
		

	}
}