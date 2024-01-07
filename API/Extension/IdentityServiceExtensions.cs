using System.Text;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extension;

public static class IdentityServiceExtensions
{
	public static IServiceCollection AddIdentityServices(this IServiceCollection services,
		IConfiguration configuration)
	{
		string? jwtTokenKey = configuration[TokenService.JWT_TOKEN_KEY_CONFIG_KEY];
		if (jwtTokenKey == null) throw new ArgumentNullException($"{TokenService.JWT_TOKEN_KEY_CONFIG_KEY} is invalid");
		
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey)),
					ValidateIssuer = false, // TODO
					ValidateAudience = false
				};
			});

		return services;
	}
}