namespace MonthlyClaimsSystem.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public string EmployeeNumber { get; set; }
        public string ClaimMonth { get; set; }
        public string ClaimType { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Pending"; // Default status
        
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public string AttachmentPath { get; set; }
        public string AttachmentName { get; set; }
        public long AttachmentSize { get; set; }

        public List<ClaimStatusLog> StatusLogs { get; set; }

        public User Lecturer { get; set; }
    }
}