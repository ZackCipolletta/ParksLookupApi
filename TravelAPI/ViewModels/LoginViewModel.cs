using System.ComponentModel.DataAnnotations;

namespace ParksLookupApi.ViewModels
{
  public class LoginViewModel
  {
    public string UserName { get; set; }
    public string Password { get; set; }
  }
}