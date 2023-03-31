using System.ComponentModel.DataAnnotations;

namespace ParksLookup
{
  public class AuthenticationRequest
  {
    [Required]
    public string userName { get; set; }
    [Required]
    public string Password { get; set; }
  }
}