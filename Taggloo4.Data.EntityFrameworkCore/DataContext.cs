using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Data.EntityFrameworkCore.Identity;
using Taggloo4.Model;
using Taggloo4.Model.Exceptions;

namespace Taggloo4.Data.EntityFrameworkCore;

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
	/// Summary of Words in Dictionaries.
	/// </summary>
	public DbSet<WordsInDictionariesSummary> WordsInDictionariesSummaries { get; set; }

	/// <summary>
	/// Words in Dictionaries.
	/// </summary>
	public DbSet<WordInDictionary> WordsInDictionaries { get; set; }

	/// <summary>
	/// Content Types for Dictionaries.
	/// </summary>
	public DbSet<ContentType> ContentTypes { get; set; }

	/// <summary>
	/// Dictionaries with Content Type and Language.
	/// </summary>
	public DbSet<DictionaryWithContentTypeAndLanguage> DictionariesWithContentTypeAndLanguage { get; set; }
	
	/// <summary>
	/// Summary of Dictionaries.
	/// </summary>
	public DbSet<DictionariesSummary> DictionariesSummaries { get; set; }
	
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
		
		ConfigureDictionaries(builder);
		SeedContentTypes(builder);
		ConfigureIdentity(builder);
		ConfigureLanguages(builder);
		ConfigureWords(builder);
		ConfigureWordTranslations(builder);
		ConfigurePhrases(builder);
		ConfigurePhraseTranslations(builder);


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
		
		// translation configuration

		builder.Entity<TranslatorConfiguration>().HasData(new TranslatorConfiguration()
		{
			Key = "WordTranslator",
			Priority = 1,
			IsEnabled = true,
			NumberOfItemsInSummary = 6
		});
		
		// views

		builder.Entity<WordsInDictionariesSummary>()
			.ToView("vw_WordsInDictionariesSummary")
			.HasKey(t => t.DictionaryId);

		builder.Entity<WordInDictionary>()
			.ToView("vw_WordsInDictionaries")
			.HasKey(t => t.WordId);

	}

	private void ConfigurePhraseTranslations(ModelBuilder builder)
	{
		builder.Entity<Dictionary>()
			.HasMany(d => d.PhraseTranslations)
			.WithOne(pt => pt.Dictionary)
			.HasForeignKey(wt => wt.DictionaryId)
			.IsRequired();
	}

	private void ConfigurePhrases(ModelBuilder builder)
	{
		builder.Entity<Phrase>()
			.HasMany(word => word.Dictionaries)
			.WithMany(dictionary => dictionary.Phrases)
			.LeftNavigation.ForeignKey!.DeleteBehavior = DeleteBehavior.Restrict; // do not delete dictionary if word is deleted
	}

	private void ConfigureWordTranslations(ModelBuilder builder)
	{
		builder.Entity<Dictionary>()
			.HasMany(d => d.WordTranslations)
			.WithOne(wt => wt.Dictionary)
			.HasForeignKey(wt => wt.DictionaryId)
			.IsRequired();
	}

	private void ConfigureWords(ModelBuilder builder)
	{
		builder.Entity<Word>()
			.HasMany(word => word.Dictionaries)
			.WithMany(dictionary => dictionary.Words)
			.LeftNavigation.ForeignKey!.DeleteBehavior = DeleteBehavior.Restrict; // do not delete dictionary if word is deleted
	}

	private void ConfigureLanguages(ModelBuilder builder)
	{
		builder.Entity<Language>()
			.HasMany(l => l.Dictionaries)
			.WithOne(d => d.Language)
			.HasForeignKey(d => d.IetfLanguageTag)
			.IsRequired();
	}

	private void ConfigureIdentity(ModelBuilder builder)
	{
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

	}

	private void SeedContentTypes(ModelBuilder builder)
	{
		builder.Entity<ContentType>().HasData(new ContentType()
		{
			Id = 1,
			Controller = "words",
			ContentTypeKey = "Word",
			NamePlural = "Words",
			NameSingular = "Word",
			ContentTypeManagerDotNetAssemblyName = "Taggloo4.Translation",
			ContentTypeManagerDotNetTypeName = "Taggloo4.Translation.ContentTypes.WordsContentTypeManager",
		});
		builder.Entity<ContentType>().HasData(new ContentType()
		{
			Id = 2,
			Controller = "wordTranslations",
			ContentTypeKey = "WordTranslation",
			NamePlural = "Word Translations",
			NameSingular = "Word Translation",
			ContentTypeManagerDotNetAssemblyName = "Taggloo4.Translation",
			ContentTypeManagerDotNetTypeName = "Taggloo4.Translation.ContentTypes.WordTranslationsContentTypeManager",
		});
		builder.Entity<ContentType>().HasData(new ContentType()
		{
			Id = 3,
			Controller = "phraseTranslations",
			ContentTypeKey = "PhraseTranslation",
			NamePlural = "Phrase Translations",
			NameSingular = "Phrase Translation",
			ContentTypeManagerDotNetAssemblyName = "Taggloo4.Translation",
			ContentTypeManagerDotNetTypeName = "Taggloo4.Translation.ContentTypes.PhraseTranslationsContentTypeManager",
		});
		builder.Entity<ContentType>().HasData(new ContentType()
		{
			Id = 4,
			Controller = "phrases",
			ContentTypeKey = "Phrase",
			NamePlural = "Phrases",
			NameSingular = "Phrase",
			ContentTypeManagerDotNetAssemblyName = "Taggloo4.Translation",
			ContentTypeManagerDotNetTypeName = "Taggloo4.Translation.ContentTypes.PhrasesContentTypeManager",
		});
	}

	private void ConfigureDictionaries(ModelBuilder builder)
	{
		builder.Entity<Dictionary>()
			.HasOne(dictionary => dictionary.ContentType)
			.WithMany(contentType => contentType.Dictionaries)
			.HasForeignKey(d => d.ContentTypeId)
			.IsRequired(); 
		
		builder.Entity<DictionaryWithContentTypeAndLanguage>()
			.ToView("vw_DictionariesWithContentTypeAndLanguage")
			.HasKey(t => t.DictionaryId);
		
		builder.Entity<DictionariesSummary>()
			.ToView("vw_DictionariesSummary")
			.HasKey(t => new
			{
				t.NumberOfDictionaries,
				t.NumberOfContentTypes,
				t.NumberOfLanguagesInDictionaries,
			});
		
	}
}