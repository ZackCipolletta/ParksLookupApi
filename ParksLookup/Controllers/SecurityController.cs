using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ParksLookupApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using ParksLookupApi.ViewModels;

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

    [HttpPost("createUser")]

    public async Task<ActionResult> CreateUser([FromBody] UserRegistrationModel model)
    {
      var user = new User
      {
        Email = model.Email,
        UserName = model.UserName
      };

      var result = await _userManager.CreateAsync(user, model.Password);

      if (!result.Succeeded)
      {
        return BadRequest(new { message = "User creation failed", errors = result.Errors });
      }

      return Ok(new { message = "User created successfully" });
    }

    [AllowAnonymous]
    [HttpPost("createToken")]
    public async Task<ActionResult<string>> CreateToken([FromBody] LoginViewModel login)
    {
      var user = await _userManager.FindByNameAsync(login.UserName);

      if (string.IsNullOrEmpty(login.UserName))
      {
        return BadRequest("Username cannot be null or empty.");
      }

      if (user == null)
      {
        return Unauthorized(new { message = "Invalid username or password" });
      }

      var isPasswordValid = await _userManager.CheckPasswordAsync(user, login.Password);

      if (!isPasswordValid)
      {
        return Unauthorized(new { message = "Invalid username or password" });
      }

      var authClaims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, login.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

      var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

      var token = new JwtSecurityToken(
          issuer: _configuration["JWT:ValidIssuer"],
          audience: _configuration["JWT:ValidAudience"],
          expires: DateTime.UtcNow.AddHours(1),
          claims: authClaims,
          signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
      );

      var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

      return Ok(tokenString);
    }

  }

}
