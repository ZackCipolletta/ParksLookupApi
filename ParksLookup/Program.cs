using System.Text;
using ParksLookupApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ThisIsMySecretKeyForKeepingThingsSafeItIsSupeSuperSecret")),
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
  };
  options.Events = new JwtBearerEvents
  {
    OnAuthenticationFailed = context =>
    {
      if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
      {
        context.Response.Headers.Add("Token-Expired", "true");
      }
      return Task.CompletedTask;
    },
    OnChallenge = context =>
    {
      if (context.AuthenticateFailure != null)
      {
        context.Response.Headers.Add("Token-Expired", "true");
      }
      return Task.CompletedTask;
    }
  };
});
builder.Services.AddControllers();

builder.Services.AddDbContext<ParksLookupApiContext>(
dbContextOptions => dbContextOptions.UseMySql(
builder.Configuration["ConnectionStrings:DefaultConnection"],
ServerVersion.AutoDetect(builder.Configuration["ConnectionStrings:DefaultConnection"])
)
);

builder.Services.AddIdentity<User, IdentityRole>()
.AddEntityFrameworkStores<ParksLookupApiContext>()
.AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
else
{
  app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
