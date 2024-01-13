using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Contract;
using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
	private readonly IConfiguration _configuration;
	private readonly UserManager<AppUser> _userManager;
	private readonly SymmetricSecurityKey _symmetricSecurityKey;

	/// <summary>
	/// Returns the key used to locate the configuration for the token key for JWT tokens
	/// </summary>
	public const string JWT_TOKEN_KEY_CONFIG_KEY = "Security:JwtPolicy:TokenKey";
	
	public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
	{
		_configuration = configuration;
		_userManager = userManager;
		string? jwtTokenKey = _configuration[JWT_TOKEN_KEY_CONFIG_KEY];
		if (jwtTokenKey == null) throw new ArgumentNullException($"{JWT_TOKEN_KEY_CONFIG_KEY} is invalid");
		
		_symmetricSecurityKey =
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey));
	}
	
	public async Task<string> CreateToken(AppUser user)
	{
		List<Claim> claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
			// could use claims to identify roles
		};

		IList<string> roles = await _userManager.GetRolesAsync(user);
		claims.AddRange(roles.Select(q=>new Claim(ClaimTypes.Role, q)));
		
		SigningCredentials credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

		string securityJwtPolicyExpirationSecsConfigKey = "Security:JwtPolicy:ExpirationSecs";
		int jwtExpirationSecs=_configuration.GetValue<int>(securityJwtPolicyExpirationSecsConfigKey);

		SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.Now.AddSeconds(jwtExpirationSecs),
			SigningCredentials = credentials
		};

		JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

		SecurityToken? token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);

		return jwtSecurityTokenHandler.WriteToken(token);
		
	}
}