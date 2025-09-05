using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;

namespace MonthlyClaimsSystem.Data
{
    public class ClaimDbContext : DbContext
    {
        public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimStatusLog> ClaimStatusLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Link Claims.EmployeeNumber to Users.EmployeeNumber
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.User)
                .WithMany(u => u.Claims)
                .HasForeignKey(c => c.EmployeeNumber)
                .HasPrincipalKey(u => u.EmployeeNumber);

            // Optional: link ClaimStatusLog.ChangedBy to Users.EmployeeNumber
            //modelBuilder.Entity<ClaimStatusLog>()
                //.HasOne<User>()
                //.WithMany()
                //.HasForeignKey(log => log.ChangedBy)
                //.HasPrincipalKey(u => u.EmployeeNumber);
        }
    }
}

