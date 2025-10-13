using System.ComponentModel.DataAnnotations.Schema;

namespace MonthlyClaimsSystem.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public string EmployeeNumber { get; set; }
        public string ClaimType { get; set; }
        public string ClaimMonth { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; }
        public string DocumentPath { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

}
