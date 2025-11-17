using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;

public class ClaimDbContext : DbContext
{
    #region Public Constructors

    public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options)
    {
    }

    #endregion Public Constructors

    #region Public Properties

    public DbSet<Claim> Claims { get; set; }
    public DbSet<ClaimStatusLog> ClaimStatusLogs { get; set; }
    public DbSet<User> Users { get; set; }

    #endregion Public Properties

    #region Protected Methods

        #region Overrides of DbContext

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                //Sets LogId as the primary key for the ClaimStatusLog table.
                //Ensures each status log entry is uniquely identifiable (code gets upset without this idk man).
                modelBuilder.Entity<ClaimStatusLog>()
                    .HasKey(c => c.LogId);

                //Limits HourlyRate to 6 digits total, 2 after the decimal to match the database schema.
                //It didn't want to submit the data before adding this bc of the "precision mismatch".
                modelBuilder.Entity<Claim>()
                    .Property(c => c.HourlyRate)
                    .HasPrecision(6, 2);

                //Limits HoursWorked to 5 digits total, 2 after the decimal to match the database schema.
                //It didn't want to submit the data before adding this bc of the "precision mismatch".
                modelBuilder.Entity<Claim>()
                    .Property(c => c.HoursWorked)
                    .HasPrecision(5, 2);

                //Maps the ClaimStatusLog entity to a table (in the database) with the same name.
                //Ensures the table name matches the entity name (it kept adding an s to the end, no clue why but this fixed it).
                modelBuilder.Entity<ClaimStatusLog>().ToTable("ClaimStatusLog");

                //Links each claim to the lecturer who submitted it.
                //Establishes a relationship between claims and users, again using EmployeeNumber for consistency (its a pk, makes sense to use it). 
                modelBuilder.Entity<Claim>()
                    .HasOne(c => c.Lecturer)
                    .WithMany()
                    .HasForeignKey(c => c.EmployeeNumber)
                    .HasPrincipalKey(u => u.EmployeeNumber);
            }

        #endregion Overrides of DbContext

    #endregion Protected Methods
}