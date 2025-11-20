namespace MonthlyClaimsSystem.ViewModels;

public class InvoiceViewModel
{
    public string EmployeeNumber { get; set; }
    public string LecturerName { get; set; }
    public string ClaimMonth { get; set; }
    public List<InvoiceLineItem> LineItems { get; set; }
    public decimal GrandTotal { get; set; }
}

public class InvoiceLineItem
{
    public string ClaimMonth { get; set; }
    public string ClaimType { get; set; }
    public decimal Hours { get; set; }
    public decimal Rate { get; set; }
    public decimal Total { get; set; }
}
