using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParksLookupApi.Models;
using ParksLookupApi.ViewModels;
using ParksLookup.Services;

namespace ParksLookupApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly ParksLookupApiContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ParksLookupApiContext db, JwtService jwtService)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _jwtService = jwtService;
      _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      ApplicationUser user = new ApplicationUser { UserName = model.UserName };
      IdentityResult result = await _userManager.CreateAsync(user, model.Password);

      if (result.Succeeded)
      {
        return Ok();
      }
      else
      {
        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
        return BadRequest(ModelState);
      }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: true, lockoutOnFailure: false);
      if (result.Succeeded)
      {
        return Ok();
      }
      else
      {
        ModelState.AddModelError("", "There is something wrong with your email or username. Please try again.");
        return BadRequest(ModelState);
      }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      return Ok();
    }

    // // POST: api/Users/BearerToken
    // [HttpPost("BearerToken")]
    // public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
    // {
    //   if (!ModelState.IsValid)
    //   {
    //     return BadRequest("Bad credentials");
    //   }

    //   var user = await _userManager.FindByNameAsync(request.UserName);

    //   if (user == null)
    //   {
    //     return BadRequest("Bad credentials");
    //   }

    //   var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

    //   if (!isPasswordValid)
    //   {
    //     return BadRequest("Bad credentials");
    //   }

    //   var token = _jwtService.CreateToken(user);

    //   return Ok(token);
    // }
  }
}