﻿using Microsoft.EntityFrameworkCore;
using Serilog;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Data.EntityFrameworkCore.Identity;
using Taggloo4.Repository;
using Taggloo4.Translation;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Data;
using Taggloo4.Web.Services;
using Taggloo4.Web.Translation;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Taggloo4.Web.Extension;

/// <summary>
/// Extension methods for Application-level services.
/// </summary>
public static class ApplicationServiceExtensions
{
	/// <summary>
	/// Adds Application-level services to the <seealso cref="IServiceCollection"/> during initialisation.
	/// </summary>
	/// <param name="services"><seealso cref="IServiceCollection"/> that will contain all services for service resolution.</param>
	/// <param name="configuration">The active <seealso cref="IConfiguration"/>.</param>
	/// <returns>The configured <seealso cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddApplicationService(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<DataContext>(opt =>
		{
			opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
		});
//services.AddCors();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<ILanguageRepository, LanguageRepository>();
		services.AddScoped<IDictionaryRepository, DictionaryRepository>();
		services.AddScoped<IContentTypeRepository, ContentTypeRepository>();
		services.AddScoped<IWordRepository, WordRepository>();
		services.AddScoped<IPhraseRepository, PhraseRepository > ();
		services.AddScoped<ITranslationRepository, TranslationRepository>();
		services.AddScoped<IDatabaseManagement, DatabaseManagement>();
		services.AddScoped<IWordInPhraseRepository, WordInPhraseRepository>();
		services.AddScoped<ITranslatorConfigurationRepository, TranslatorConfigurationRepository>();
		services.AddScoped<ICommunityContentItemRepository, CommunityContentItemRepository>();
		services.AddScoped<ICommunityContentCollectionRepository, CommunityContentCollectionRepository>();
		services.AddSingleton<TranslationFactoryService, TranslationFactoryService>();
		services.AddSingleton<TranslatorConfigurationCache, TranslatorConfigurationCache>();
		
		
		
		services.AddLogging(loggingBuilder =>
			loggingBuilder.AddSerilog(dispose: true));
		
		
		return services;
	}
}