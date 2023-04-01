using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParksLookupApi.Models;
using ParksLookup.Services;

using Microsoft.AspNetCore.Authorization;
using ParksLookup;

namespace ParksLookupApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtService _jwtService;

    public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtService jwtService)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _jwtService = jwtService;
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<ApplicationUser>> PostUser(ApplicationUser user)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _userManager.CreateAsync(
          new ApplicationUser() { UserName = user.UserName, Email = user.Email },
          user.Password
      );

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      user.Password = null;
      return CreatedAtAction("GetUser", new { username = user.UserName }, user);
    }

    // GET: api/Users/username
    [HttpGet("{username}")]
    public async Task<ActionResult<ApplicationUser>> GetUser(string username)
    {
      ApplicationUser user = await _userManager.FindByNameAsync(username);

      if (user == null)
      {
        return NotFound();
      }

      return new ApplicationUser
      {
        UserName = user.UserName,
        Email = user.Email
      };
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest model)
    {
      var user = await _userManager.FindByNameAsync(model.userName);
      if (user == null)
      {
        return BadRequest(new { message = "Username or password is incorrect" });
      }

      var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
      if (!result.Succeeded)
      {
        return BadRequest(new { message = "Username or password is incorrect" });
      }

      var token = _jwtService.CreateToken(user);

      return Ok(new AuthenticationResponse { Token = token, Expiration = _jwtService.GetExpiration() });
    }
  }
}
