using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;


namespace MonthlyClaimsSystem.Data
{
   
    public class ClaimDbContext : DbContext
    {
        public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        //public DbSet<ClaimStatusLog> ClaimStatusLogs { get; set; }
    }

}
