using Microsoft.EntityFrameworkCore;
using Pito.Models;

public class LoginContext : DbContext
{
    public LoginContext(DbContextOptions<LoginContext> options) : base(options) { }

    public DbSet<Login> Logged { get; set; }
    public DbSet<TopicModel> Topics { get; set; }
    public DbSet<ThreadModel> Threads { get; set; }
    public DbSet<ReplyModel> Replies { get; set; }




}
