using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParksLookupApi.Models;
using ParksLookup.Services;

namespace ParksLookup.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;

    public UsersController(UserManager<ApplicationUser> userManager, JwtService jwtService)
    {
      _userManager = userManager;
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
    // _______________________________________________________________________________________________________________
    [HttpPost("BearerToken")]
    public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
    {
      var response = await CreateAuthToken(request, user => _jwtService.CreateToken(user));
      return Ok(response);
    }
    private async Task<ApplicationUser> Authenticate(AuthenticationRequest request)
    {
      if (!ModelState.IsValid)
      {
        return null;
      }

      var user = await _userManager.FindByNameAsync(request.userName);

      if (user == null)
      {
        return null;
      }

      var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

      if (!isPasswordValid)
      {
        return null;
      }

      return user;
    }

    private async Task<ActionResult<AuthenticationResponse>> CreateAuthToken(
        AuthenticationRequest request,
        Func<ApplicationUser, Task<string>> createToken)
    {
      var user = await Authenticate(request);

      if (user == null)
      {
        return Unauthorized();
      }

      var token = await createToken(user);
      var expiration = _jwtService.GetExpiration();

      return new AuthenticationResponse { Token = token, Expiration = expiration };
    }
  }
}