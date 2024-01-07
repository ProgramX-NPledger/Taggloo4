using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Contract;
using API.Model;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
	private readonly SymmetricSecurityKey _symmetricSecurityKey;
	
	public TokenService(IConfiguration configuration)
	{
		string? jwtTokenKey = configuration["JwtTokenKey"];
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

		SecurityTokenDescriptor tokenDescripter = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.Now.AddDays(7), // TODO configuration
			SigningCredentials = credentials
		};

		JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

		SecurityToken? token = jwtSecurityTokenHandler.CreateToken(tokenDescripter);

		return jwtSecurityTokenHandler.WriteToken(token);
		
	}
}