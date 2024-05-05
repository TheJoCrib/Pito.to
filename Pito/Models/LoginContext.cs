using Microsoft.EntityFrameworkCore;
using Pito.Models;

public class LoginContext : DbContext
{
    public DbSet<Login> Logged { get; set; }
    public DbSet<Library> libraries { get; set; }

    // Adjusted to accept DbContextOptions
    public LoginContext(DbContextOptions<LoginContext> options) : base(options)
    {
    }

}
