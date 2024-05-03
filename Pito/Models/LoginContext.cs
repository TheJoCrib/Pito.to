using Microsoft.EntityFrameworkCore;
using Pito.Models;

public class LoginContext : DbContext
{
    public DbSet<Login> Logged { get; set; }

    // Adjusted to accept DbContextOptions
    public LoginContext(DbContextOptions<LoginContext> options) : base(options)
    {
        Database.EnsureCreated();  // Note: Consider using Migrations instead of EnsureCreated for production environments
    }

}
