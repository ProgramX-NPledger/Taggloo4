﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Taggloo4.Data.EntityFrameworkCore;

#nullable disable

namespace Taggloo4.Data.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241027150608_Migrate to M-M Phrases-Dictionaries")]
    partial class MigratetoMMPhrasesDictionaries
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DictionaryPhrase", b =>
                {
                    b.Property<int>("DictionariesId")
                        .HasColumnType("int");

                    b.Property<int>("PhrasesId")
                        .HasColumnType("int");

                    b.HasKey("DictionariesId", "PhrasesId");

                    b.HasIndex("PhrasesId");

                    b.ToTable("DictionaryPhrase");
                });

            modelBuilder.Entity("DictionaryWord", b =>
                {
                    b.Property<int>("DictionariesId")
                        .HasColumnType("int");

                    b.Property<int>("WordsId")
                        .HasColumnType("int");

                    b.HasKey("DictionariesId", "WordsId");

                    b.HasIndex("WordsId");

                    b.ToTable("DictionaryWord");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("PhraseWord", b =>
                {
                    b.Property<int>("PhrasesId")
                        .HasColumnType("int");

                    b.Property<int>("WordsId")
                        .HasColumnType("int");

                    b.HasKey("PhrasesId", "WordsId");

                    b.HasIndex("WordsId");

                    b.ToTable("PhraseWord");
                });

            modelBuilder.Entity("Taggloo4.Data.EntityFrameworkCore.Identity.AppRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Taggloo4.Data.EntityFrameworkCore.Identity.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Taggloo4.Data.EntityFrameworkCore.Identity.AppUserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Taggloo4.Model.ContentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ContentTypeKey")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ContentTypeManagerDotNetAssemblyName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ContentTypeManagerDotNetTypeName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Controller")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("NamePlural")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameSingular")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("ContentTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ContentTypeKey = "Word",
                            ContentTypeManagerDotNetAssemblyName = "Taggloo4.Translation",
                            ContentTypeManagerDotNetTypeName = "Taggloo4.Translation.ContentTypes.WordsContentTypeManager",
                            Controller = "words",
                            NamePlural = "Words",
                            NameSingular = "Word"
                        },
                        new
                        {
                            Id = 2,
                            ContentTypeKey = "WordTranslation",
                            ContentTypeManagerDotNetAssemblyName = "Taggloo4.Translation",
                            ContentTypeManagerDotNetTypeName = "Taggloo4.Translation.ContentTypes.WordTranslationsContentTypeManager",
                            Controller = "wordTranslations",
                            NamePlural = "Word Translations",
                            NameSingular = "Word Translation"
                        },
                        new
                        {
                            Id = 3,
                            ContentTypeKey = "PhraseTranslation",
                            ContentTypeManagerDotNetAssemblyName = "Taggloo4.Translation",
                            ContentTypeManagerDotNetTypeName = "Taggloo4.Translation.ContentTypes.PhraseTranslationsContentTypeManager",
                            Controller = "phraseTranslations",
                            NamePlural = "Phrase Translations",
                            NameSingular = "Phrase Translation"
                        },
                        new
                        {
                            Id = 4,
                            ContentTypeKey = "Phrase",
                            ContentTypeManagerDotNetAssemblyName = "Taggloo4.Translation",
                            ContentTypeManagerDotNetTypeName = "Taggloo4.Translation.ContentTypes.PhrasesContentTypeManager",
                            Controller = "phrases",
                            NamePlural = "Phrases",
                            NameSingular = "Phrase"
                        });
                });

            modelBuilder.Entity("Taggloo4.Model.Dictionary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByUserName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("CreatedOn")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("IetfLanguageTag")
                        .IsRequired()
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("SourceUrl")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.HasKey("Id");

                    b.HasIndex("ContentTypeId");

                    b.HasIndex("IetfLanguageTag");

                    b.ToTable("Dictionaries");
                });

            modelBuilder.Entity("Taggloo4.Model.DictionaryWithContentTypeAndLanguage", b =>
                {
                    b.Property<int>("DictionaryId")
                        .HasColumnType("int");

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ContentTypeKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Controller")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedOn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IetfLanguageTag")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LanguageName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NamePlural")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameSingular")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DictionaryId");

                    b.ToTable((string)null);

                    b.ToView("vw_DictionariesWithContentTypeAndLanguage", (string)null);
                });

            modelBuilder.Entity("Taggloo4.Model.Exceptions.DictionariesSummary", b =>
                {
                    b.Property<int>("NumberOfDictionaries")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfContentTypes")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfLanguagesInDictionaries")
                        .HasColumnType("int");

                    b.HasKey("NumberOfDictionaries", "NumberOfContentTypes", "NumberOfLanguagesInDictionaries");

                    b.ToTable((string)null);

                    b.ToView("vw_DictionariesSummary", (string)null);
                });

            modelBuilder.Entity("Taggloo4.Model.Language", b =>
                {
                    b.Property<string>("IetfLanguageTag")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IetfLanguageTag");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Taggloo4.Model.Phrase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedOn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DictionaryId")
                        .HasColumnType("int");

                    b.Property<string>("ExternalId")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ThePhrase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Phrases");
                });

            modelBuilder.Entity("Taggloo4.Model.PhraseTranslation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedOn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DictionaryId")
                        .HasColumnType("int");

                    b.Property<int>("FromPhraseId")
                        .HasColumnType("int");

                    b.Property<int>("ToPhraseId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.HasIndex("FromPhraseId");

                    b.HasIndex("ToPhraseId");

                    b.ToTable("PhraseTranslations");
                });

            modelBuilder.Entity("Taggloo4.Model.ReindexingJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("FinishedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("NumberOfDictionariesProcessed")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfLanguagesProcessed")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfPhrasesProcessed")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfWordProcessed")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfWordsCreated")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfWordsInPhrasesCreated")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfWordsInPhrasesRemoved")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("StartedByUserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartedOn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ReindexingJobs");
                });

            modelBuilder.Entity("Taggloo4.Model.TranslatorConfiguration", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<int>("NumberOfItemsInSummary")
                        .HasColumnType("int");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.HasKey("Key");

                    b.ToTable("TranslatorConfigurations");

                    b.HasData(
                        new
                        {
                            Key = "WordTranslator",
                            IsEnabled = true,
                            NumberOfItemsInSummary = 6,
                            Priority = 1
                        });
                });

            modelBuilder.Entity("Taggloo4.Model.Word", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByUserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedOn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExternalId")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("TheWord")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("Taggloo4.Model.WordInDictionary", b =>
                {
                    b.Property<int?>("WordId")
                        .HasColumnType("int");

                    b.Property<int?>("AppearsInPhrasesCount")
                        .HasColumnType("int");

                    b.Property<string>("ContentTypeFriendlyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedOn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DictionaryId")
                        .HasColumnType("int");

                    b.Property<string>("DictionaryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExternalId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IetfLanguageTag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LanguageName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TheWord")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("WordId");

                    b.ToTable((string)null);

                    b.ToView("vw_WordsInDictionaries", (string)null);
                });

            modelBuilder.Entity("Taggloo4.Model.WordInPhrase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByUserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedOn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InPhraseId")
                        .HasColumnType("int");

                    b.Property<int>("Ordinal")
                        .HasColumnType("int");

                    b.Property<int>("WordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InPhraseId");

                    b.HasIndex("WordId");

                    b.ToTable("WordsInPhrases");
                });

            modelBuilder.Entity("Taggloo4.Model.WordTranslation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedOn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DictionaryId")
                        .HasColumnType("int");

                    b.Property<int>("FromWordId")
                        .HasColumnType("int");

                    b.Property<int>("ToWordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.HasIndex("FromWordId");

                    b.HasIndex("ToWordId");

                    b.ToTable("WordTranslations");
                });

            modelBuilder.Entity("Taggloo4.Model.WordsInDictionariesSummary", b =>
                {
                    b.Property<int?>("DictionaryId")
                        .HasColumnType("int");

                    b.Property<string>("DictionaryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LatestWordCreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("WordCount")
                        .HasColumnType("int");

                    b.HasKey("DictionaryId");

                    b.ToTable((string)null);

                    b.ToView("vw_WordsInDictionariesSummary", (string)null);
                });

            modelBuilder.Entity("DictionaryPhrase", b =>
                {
                    b.HasOne("Taggloo4.Model.Dictionary", null)
                        .WithMany()
                        .HasForeignKey("DictionariesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Phrase", null)
                        .WithMany()
                        .HasForeignKey("PhrasesId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DictionaryWord", b =>
                {
                    b.HasOne("Taggloo4.Model.Dictionary", null)
                        .WithMany()
                        .HasForeignKey("DictionariesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Word", null)
                        .WithMany()
                        .HasForeignKey("WordsId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Taggloo4.Data.EntityFrameworkCore.Identity.AppRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Taggloo4.Data.EntityFrameworkCore.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Taggloo4.Data.EntityFrameworkCore.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("Taggloo4.Data.EntityFrameworkCore.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PhraseWord", b =>
                {
                    b.HasOne("Taggloo4.Model.Phrase", null)
                        .WithMany()
                        .HasForeignKey("PhrasesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Word", null)
                        .WithMany()
                        .HasForeignKey("WordsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Taggloo4.Data.EntityFrameworkCore.Identity.AppUserRole", b =>
                {
                    b.HasOne("Taggloo4.Data.EntityFrameworkCore.Identity.AppRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taggloo4.Data.EntityFrameworkCore.Identity.AppUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Taggloo4.Model.Dictionary", b =>
                {
                    b.HasOne("Taggloo4.Model.ContentType", "ContentType")
                        .WithMany("Dictionaries")
                        .HasForeignKey("ContentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Language", "Language")
                        .WithMany("Dictionaries")
                        .HasForeignKey("IetfLanguageTag")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentType");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("Taggloo4.Model.PhraseTranslation", b =>
                {
                    b.HasOne("Taggloo4.Model.Dictionary", "Dictionary")
                        .WithMany("PhraseTranslations")
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Phrase", "FromPhrase")
                        .WithMany("FromTranslations")
                        .HasForeignKey("FromPhraseId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Phrase", "ToPhrase")
                        .WithMany("Translations")
                        .HasForeignKey("ToPhraseId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Dictionary");

                    b.Navigation("FromPhrase");

                    b.Navigation("ToPhrase");
                });

            modelBuilder.Entity("Taggloo4.Model.WordInPhrase", b =>
                {
                    b.HasOne("Taggloo4.Model.Phrase", "InPhrase")
                        .WithMany("HasWordsInPhrase")
                        .HasForeignKey("InPhraseId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Word", "Word")
                        .WithMany("AppearsInPhrases")
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("InPhrase");

                    b.Navigation("Word");
                });

            modelBuilder.Entity("Taggloo4.Model.WordTranslation", b =>
                {
                    b.HasOne("Taggloo4.Model.Dictionary", "Dictionary")
                        .WithMany("WordTranslations")
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Word", "FromWord")
                        .WithMany("FromTranslations")
                        .HasForeignKey("FromWordId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Taggloo4.Model.Word", "ToWord")
                        .WithMany("ToTranslations")
                        .HasForeignKey("ToWordId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Dictionary");

                    b.Navigation("FromWord");

                    b.Navigation("ToWord");
                });

            modelBuilder.Entity("Taggloo4.Data.EntityFrameworkCore.Identity.AppRole", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Taggloo4.Data.EntityFrameworkCore.Identity.AppUser", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Taggloo4.Model.ContentType", b =>
                {
                    b.Navigation("Dictionaries");
                });

            modelBuilder.Entity("Taggloo4.Model.Dictionary", b =>
                {
                    b.Navigation("PhraseTranslations");

                    b.Navigation("WordTranslations");
                });

            modelBuilder.Entity("Taggloo4.Model.Language", b =>
                {
                    b.Navigation("Dictionaries");
                });

            modelBuilder.Entity("Taggloo4.Model.Phrase", b =>
                {
                    b.Navigation("FromTranslations");

                    b.Navigation("HasWordsInPhrase");

                    b.Navigation("Translations");
                });

            modelBuilder.Entity("Taggloo4.Model.Word", b =>
                {
                    b.Navigation("AppearsInPhrases");

                    b.Navigation("FromTranslations");

                    b.Navigation("ToTranslations");
                });
#pragma warning restore 612, 618
        }
    }
}