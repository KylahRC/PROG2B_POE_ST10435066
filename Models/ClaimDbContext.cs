using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;

public class ClaimDbContext : DbContext
{
    public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Claim> Claims { get; set; }

    public DbSet<ClaimStatusLog> ClaimStatusLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClaimStatusLog>()
            .HasKey(c => c.LogId);

        modelBuilder.Entity<Claim>()
            .Property(c => c.HourlyRate)
            .HasPrecision(6, 2);

        modelBuilder.Entity<Claim>()
            .Property(c => c.HoursWorked)
            .HasPrecision(5, 2);

        modelBuilder.Entity<ClaimStatusLog>().ToTable("ClaimStatusLog");

        modelBuilder.Entity<ClaimStatusLog>()
            .HasOne(log => log.ChangedByUser)
            .WithMany()
            .HasForeignKey(log => log.ChangedBy)
            .HasPrincipalKey(user => user.EmployeeNumber);

        modelBuilder.Entity<Claim>()
            .HasOne(c => c.Lecturer)
            .WithMany()
            .HasForeignKey(c => c.EmployeeNumber)
            .HasPrincipalKey(u => u.EmployeeNumber);
    }
}