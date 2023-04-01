using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ParksLookupApi.Models
{
  public class User : IdentityUser
  {
    public string Password { get; set; }
  }
}
