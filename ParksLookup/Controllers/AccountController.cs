using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParksLookupApi.Models;
using ParksLookupApi.ViewModels;
using ParksLookup.Services;
using ParksLookup;

namespace ParksLookupApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;


    public AccountController(UserManager<ApplicationUser> userManager, JwtService jwtService)
    {
      _userManager = userManager;
      _jwtService = jwtService;
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationUser>> PostUser(ApplicationUser applicationUser)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _userManager.CreateAsync(
          applicationUser,
          applicationUser.PasswordHash
      );

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      applicationUser.PasswordHash = null;
      return CreatedAtAction("GetUser", new { username = applicationUser.UserName }, applicationUser);
    }

    // POST: api/Account/BearerToken
    [HttpPost("BearerToken")]
    public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest("Bad credentials");
      }

      var user = await _userManager.FindByNameAsync(request.userName);

      if (user == null)
      {
        return BadRequest("Bad credentials");
      }

      var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

      if (!isPasswordValid)
      {
        return BadRequest("Bad credentials");
      }

      var token = _jwtService.CreateToken(user);

      return Ok(token);
    }
  }
}