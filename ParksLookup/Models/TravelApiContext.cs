using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParksLookupApi.Models;

namespace ParksLookupApi.Models
{
  public class ParksLookupApiContext : IdentityDbContext<User>
  {
    public DbSet<Park> Parks { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public ParksLookupApiContext(DbContextOptions<ParksLookupApiContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      // Configure your model here
    }
  }
}