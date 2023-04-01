using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using ParksLookupApi.Models;

namespace ParksLookup.Services
{
  public class JwtService
  {
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public string CreateToken(ApplicationUser user)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = System.Text.Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
          new Claim(JwtRegisteredClaimNames.Email, user.Email),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
        Expires = GetExpiration(),
        SigningCredentials = new SigningCredentials(
          new SymmetricSecurityKey(key),
          SecurityAlgorithms.HmacSha256Signature
        )
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }

    public DateTime GetExpiration()
    {
      return DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpirationInMinutes"]));
    }
  }
}
