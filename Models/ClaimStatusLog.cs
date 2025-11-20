namespace MonthlyClaimsSystem.Models
{
    public class ClaimStatusLog
    {
        public int? LogId { get; set; }
        public int? ClaimId { get; set; }
        public string? ChangedBy { get; set; } 
        public string? NewStatus { get; set; }
        public DateTime ChangeDate { get; set; }
        public string? Reason { get; set; }
        public bool IsSystemChange { get; set; }
    }
}