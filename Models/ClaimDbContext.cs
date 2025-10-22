using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;

public class ClaimDbContext : DbContext
{
    public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Claim> Claims { get; set; }
    public DbSet<ClaimStatusLog> ClaimStatusLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClaimStatusLog>()
            .HasKey(c => c.LogId); // Tell EF this is the primary key

        // Optional: configure other entities here
    }
}
