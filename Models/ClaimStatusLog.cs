namespace MonthlyClaimsSystem.Models
{
    public class ClaimStatusLog
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string Status { get; set; } // e.g. "Denied"
        public string Reason { get; set; } // e.g. "Missing supporting documents"
        public DateTime ChangedAt { get; set; }

        public Claim Claim { get; set; }
    }

}

