using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Contract;
using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

/// <summary>
/// Service for generation of JWT Tokens.
/// </summary>
public class TokenService : ITokenService
{
	private readonly IConfiguration _configuration;
	private readonly UserManager<AppUser> _userManager;
	private readonly SymmetricSecurityKey _symmetricSecurityKey;

	/// <summary>
	/// Returns the key used to locate the configuration for the token key for JWT tokens
	/// </summary>
	public const string JWT_TOKEN_KEY_CONFIG_KEY = "Security:JwtPolicy:TokenKey";
	
	/// <summary>
	/// Constructor for instantiating the object with required configuration.
	/// </summary>
	/// <param name="configuration">The current <seealso cref="IConfiguration"/>.</param>
	/// <param name="userManager">The <seealso cref="UserManager{TUser}"/> implementation.</param>
	/// <exception cref="ArgumentNullException">Thrown if the <seealso cref="JWT_TOKEN_KEY_CONFIG_KEY"/> configuration is not specified.</exception>
	public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
	{
		_configuration = configuration;
		_userManager = userManager;
		string? jwtTokenKey = _configuration[JWT_TOKEN_KEY_CONFIG_KEY];
		if (jwtTokenKey == null) throw new ArgumentNullException($"{JWT_TOKEN_KEY_CONFIG_KEY} is invalid");
		
		_symmetricSecurityKey =
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey));
	}
	
	/// <summary>
	/// Create JWT Token.
	/// </summary>
	/// <param name="user">The <seealso cref="AppUser"/> to generate the Token for.</param>
	/// <returns>A <see cref="string"/> containing the generated Token.</returns>
	/// <exception cref="ArgumentNullException">Thrown if UserName property is null.</exception>
	public async Task<string> CreateToken(AppUser user)
	{
		if (user.UserName == null) throw new ArgumentNullException("user.UserName");
		
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