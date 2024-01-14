using API.Contract;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extension;

public static class ApplicationServiceExtensions
{
	public static IServiceCollection AddApplicationService(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<DataContext>(opt =>
		{
			opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
		});
//services.AddCors();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<ILanguageRepository, LanguageRepository>();
		
		return services;
	}
}