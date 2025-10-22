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

        modelBuilder.Entity<Claim>()
    .Property(c => c.HourlyRate)
    .HasPrecision(6, 2); // Matches SQL schema

        modelBuilder.Entity<Claim>()
            .Property(c => c.HoursWorked)
            .HasPrecision(5, 2); // Matches SQL schema

        // Optional: configure other entities here
    }

}
