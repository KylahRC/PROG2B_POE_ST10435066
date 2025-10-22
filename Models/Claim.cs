using System.ComponentModel.DataAnnotations.Schema;

namespace MonthlyClaimsSystem.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public string EmployeeNumber { get; set; } // Foreign key to User
        public string ClaimMonth { get; set; }
        public string ClaimType { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } = "Pending"; // Default status
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }

}
