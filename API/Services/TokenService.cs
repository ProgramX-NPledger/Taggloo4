using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Contract;
using API.Model;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
	private readonly IConfiguration _configuration;
	private readonly SymmetricSecurityKey _symmetricSecurityKey;
	
	public TokenService(IConfiguration configuration)
	{
		_configuration = configuration;
		string? jwtTokenKey = _configuration["JwtTokenKey"];
		if (jwtTokenKey == null) throw new ArgumentNullException("jwtTokenKey is null");
		
		_symmetricSecurityKey =
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey));
	}
	
	public string CreateToken(AppUser user)
	{
		List<Claim> claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
			// could use claims to identify roles
		};

		SigningCredentials credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

		string securityJwtPolicyExpirationSecsConfigKey = "Security:JwtPolicy:ExpirationSecs";
		int jwtExpirationSecs=_configuration.GetValue<int>(securityJwtPolicyExpirationSecsConfigKey);

		SecurityTokenDescriptor tokenDescripter = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.Now.AddSeconds(jwtExpirationSecs),
			SigningCredentials = credentials
		};

		JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

		SecurityToken? token = jwtSecurityTokenHandler.CreateToken(tokenDescripter);

		return jwtSecurityTokenHandler.WriteToken(token);
		
	}
}