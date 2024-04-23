using Microsoft.EntityFrameworkCore;
using Pito.Models;

public class LoginContext : DbContext
{
    public DbSet<Login> Logged { get; set; }

    public LoginContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=Furkan\\SQLEXPRESS;Database=PitoDB;User Id=KarasuDB;Password=Karasu198408;TrustServerCertificate=True;MultipleActiveResultSets=True;");
    }
}
