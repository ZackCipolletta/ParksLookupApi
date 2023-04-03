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

  public static class ParksLookupApiContextFactory
  {
    public static ParksLookupApiContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<ParksLookupApiContext>();
      optionsBuilder.UseMySql("Server=localhost;Port=3306;database=ParksLookup;uid=root;pwd=epicodus", new MySqlServerVersion(new Version(8, 0, 26)));

      return new ParksLookupApiContext(optionsBuilder.Options);
    }
  }
}
