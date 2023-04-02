using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ParksLookupApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ParksLookupApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SecurityController : ControllerBase
  {
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public SecurityController(UserManager<User> userManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationModel userRegistrationModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = new User
      {
        UserName = userRegistrationModel.UserName,
        Email = userRegistrationModel.Email
      };

      var result = await _userManager.CreateAsync(user, userRegistrationModel.Password);

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      return Ok();
    }

[AllowAnonymous]
[HttpPost("createToken")]
public async Task<IActionResult> CreateToken(string username, string password)
{
  if (string.IsNullOrEmpty(username))
  {
    return BadRequest("Username is required.");
  }

  var userExists = await _userManager.FindByNameAsync(username);

  if (userExists != null && await _userManager.CheckPasswordAsync(userExists, password))
  {
    var authClaims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

    var secretKey = _configuration["JWT:Key"];
    if (string.IsNullOrEmpty(secretKey))
    {
      return BadRequest("Invalid JWT secret key.");
    }

    var authSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

    var token = new JwtSecurityToken(
        issuer: _configuration["JWT:ValidIssuer"],
        audience: _configuration["JWT:ValidAudience"],
        expires: DateTime.UtcNow.AddHours(1),
        claims: authClaims,
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
    );

    return Ok(new
    {
      token = new JwtSecurityTokenHandler().WriteToken(token),
      expiration = token.ValidTo
    });
  }

  return Unauthorized();
}



  }
}
