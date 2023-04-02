using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenValidationMiddleware
{
  private readonly RequestDelegate _next;

  public JwtTokenValidationMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
      if (!context.Request.Path.Value.Contains("/CreateToken")) // exclude CreateToken action
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Authorization header not found.");
        return;
      }
    }

    var bearerToken = context.Request.Headers["Authorization"].ToString().Split(' ')[1];
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("ThisIsMySecretKeyForKeepingThingsSafeItIsSupeSuperSecret");

    try
    {
      tokenHandler.ValidateToken(bearerToken, new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true,
        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters parameters) =>
        {
          if (expires != null && expires <= DateTime.UtcNow)
          {
            return false; // token is expired
          }
          return true;
        }
      }, out SecurityToken validatedToken);

      await _next(context);
    }
    catch (Exception ex)
    {
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      await context.Response.WriteAsync(ex.Message);
    }
  }
}
