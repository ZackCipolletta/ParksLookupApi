using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ParksLookupApi.Models;

namespace ParksLookupApi.Controllers
{

  [Route("api/[controller]")]
  [ApiController]
  public class SecurityController : ControllerBase
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public SecurityController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("createToken")]
    public async Task<IActionResult> CreateToken(User user)
    {
      var result = await _signInManager.PasswordSignInAsync(user.UserName, user.Password, false, false);

      if (result.Succeeded)
      {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(new[]
            {
                        new Claim("Id", Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }),
          Expires = DateTime.UtcNow.AddMinutes(5),
          Issuer = issuer,
          Audience = audience,
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Ok(jwtToken);
      }

      return Unauthorized();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
      var result = await _userManager.CreateAsync(user, user.Password);

      if (result.Succeeded)
      {
        return Ok("User created successfully!");
      }

      return BadRequest(result.Errors);
    }
  }
}
