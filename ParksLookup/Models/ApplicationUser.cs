using Microsoft.AspNetCore.Identity;

namespace ParksLookupApi.Models
{
  public class ApplicationUser : IdentityUser
  {
    public string Password { get; set; }
  }
}
