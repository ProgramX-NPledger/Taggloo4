using System.Text;
using API.Data;
using API.Model;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extension;

/// <summary>
/// Extension methods for Identity services.
/// </summary>
public static class IdentityServiceExtensions
{
	/// <summary>
	/// Adds authentication services to the application service resolver.
	/// </summary>
	/// <param name="services"><seealso cref="IServiceCollection"/> representing the services used by the application.</param>
	/// <param name="configuration">Active <seealso cref="IConfiguration"/>.</param>
	/// <returns>The configured <seealso cref="IServiceCollection"/>.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <seealso cref="TokenService.JWT_TOKEN_KEY_CONFIG_KEY"/> configuration is not defined.</exception>
	public static IServiceCollection AddIdentityServices(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddIdentityCore<AppUser>(options =>
			{
				options.Password.RequireNonAlphanumeric = false;
				//options.User.RequireUniqueEmail = true;
			})
			.AddSignInManager<SignInManager<AppUser>>()
			.AddRoles<AppRole>()
			.AddRoleManager<RoleManager<AppRole>>()
			.AddEntityFrameworkStores<DataContext>();
		
		string? jwtTokenKey = configuration[TokenService.JWT_TOKEN_KEY_CONFIG_KEY];
		if (jwtTokenKey == null) throw new ArgumentNullException($"{TokenService.JWT_TOKEN_KEY_CONFIG_KEY} is invalid");
		
		services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey)),
					ValidateIssuer = false, // TODO
					ValidateAudience = false
				};
			}).AddCookie("Identity.Application", options =>
			{
				options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
				options.LoginPath = "/account/login";
				options.AccessDeniedPath = "/notpermitted";
				options.SlidingExpiration = true;

			});

		
		return services;
	}
}