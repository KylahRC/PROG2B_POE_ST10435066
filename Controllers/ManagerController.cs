using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;
using MonthlyClaimsSystem.ViewModels;


public class ManagerController : Controller
{
    private readonly ClaimDbContext _context;

    public ManagerController(ClaimDbContext context)
    {
        _context = context;
    }

    public IActionResult Manager_Dashboard()
    {

        var rawUsername = HttpContext.Session.GetString("Username") ?? "Manager";

        string displayName = "Manager";
        if (!string.IsNullOrEmpty(rawUsername) && rawUsername.Length > 1)
        {
            // Remove the first character (initial)
            var surnamePart = rawUsername.Substring(1);

            // Capitalize the first letter of the surname
            displayName = char.ToUpper(surnamePart[0]) + surnamePart.Substring(1).ToLower();
        }

        ViewBag.Username = displayName;

        return View();
    }

    public IActionResult Manager_Report(string[] status, string[] employeeNumber, string[] claimMonth, string[] claimType)
    {
        var claimsQuery = _context.Claims.Include(c => c.Lecturer).AsQueryable();

        // Filters
        if (status != null && status.Any())
            claimsQuery = claimsQuery.Where(c => status.Contains(c.Status));

        if (employeeNumber != null && employeeNumber.Any())
            claimsQuery = claimsQuery.Where(c => employeeNumber.Contains(c.EmployeeNumber));

        if (claimMonth != null && claimMonth.Any())
            claimsQuery = claimsQuery.Where(c => claimMonth.Contains(c.ClaimMonth));

        if (claimType != null && claimType.Any())
            claimsQuery = claimsQuery.Where(c => claimType.Contains(c.ClaimType));


        var claims = claimsQuery.ToList();

        // Totals
        var approvedTotal = _context.Claims.Where(c => c.Status == "Approved").Sum(c => c.HoursWorked * c.HourlyRate);
        var deniedTotal = _context.Claims.Where(c => c.Status == "Denied").Sum(c => c.HoursWorked * c.HourlyRate);
        var pendingTotal = _context.Claims.Where(c => c.Status == "Pending").Sum(c => c.HoursWorked * c.HourlyRate);

        // Get lecturers list
        var lecturers = _context.Users.Where(u => u.Role == "Lecturer").ToList();
        ViewBag.Lecturers = lecturers;

        ViewBag.ApprovedTotal = approvedTotal;
        ViewBag.DeniedTotal = deniedTotal;
        ViewBag.PendingTotal = pendingTotal;
        ViewBag.Claims = claims;

        return View();
    }

    public IActionResult Manager_GenerateInvoices()
    {
        var currentMonthName = DateTime.Now.ToString("MMMM"); // e.g. "November"

        var approvedTotalForCurrentMonth = _context.Claims
            .Where(c => c.Status == "Approved" && c.ClaimMonth == currentMonthName)
            .Sum(c => c.HoursWorked * c.HourlyRate);

        var approvedTotalForYear = _context.Claims
            .Where(c => c.Status == "Approved" && c.ClaimMonth != currentMonthName)
            .Sum(c => c.HoursWorked * c.HourlyRate);

        var pendingTotalForCurrentMonth = _context.Claims
            .Where(c => c.Status == "Pending" && c.ClaimMonth == currentMonthName)
            .Sum(c => c.HoursWorked * c.HourlyRate);


        // Totals
        //var approvedTotal = _context.Claims.Where(c => c.Status == "Approved").Sum(c => c.HoursWorked * c.HourlyRate);
        //var deniedTotal = _context.Claims.Where(c => c.Status == "Denied").Sum(c => c.HoursWorked * c.HourlyRate);
        //var pendingTotal = _context.Claims.Where(c => c.Status == "Pending").Sum(c => c.HoursWorked * c.HourlyRate);

        ViewBag.ApprovedTotal = approvedTotalForCurrentMonth;
        ViewBag.DeniedTotal = approvedTotalForYear;
        ViewBag.PendingTotal = pendingTotalForCurrentMonth;

        var approvedClaims = _context.Claims
            .Include(c => c.Lecturer)
            .Where(c => c.Status == "Approved")
            .ToList();

        var invoices = approvedClaims
            .GroupBy(c => new { c.EmployeeNumber, c.ClaimMonth })
            .Select(g => new InvoiceViewModel
            {
                EmployeeNumber = g.Key.EmployeeNumber,
                LecturerName = g.First().Lecturer != null
                    ? g.First().Lecturer.Name + " " + g.First().Lecturer.Surname
                    : "Unknown",
                ClaimMonth = g.Key.ClaimMonth,
                LineItems = g.Select(c => new InvoiceLineItem
                {
                    ClaimMonth = c.ClaimMonth,
                    ClaimType = c.ClaimType,
                    Hours = c.HoursWorked,
                    Rate = c.HourlyRate,
                    Total = c.HoursWorked * c.HourlyRate
                }).ToList(),
                GrandTotal = g.Sum(c => c.HoursWorked * c.HourlyRate)
            })
            .ToList();

        return View(invoices);
    }


    // GET: Manager/EditPersonalDetails
    public IActionResult Manager_ChooseUserToEdit()
    {
        // Get all users for dropdown
        var users = _context.Users
            .Select(u => new { u.EmployeeNumber, DisplayName = u.Name + " " + u.Surname })
            .ToList();

        ViewBag.UsersDropdown = new SelectList(users, "EmployeeNumber", "DisplayName");
        return View();
    }

    // POST: Manager/EditPersonalDetails
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Manager_ChooseUserToEdit(string employeeNumber)
    {
        var user = _context.Users.FirstOrDefault(u => u.EmployeeNumber == employeeNumber);
        if (user == null)
        {
            return NotFound();
        }

        // Pass the selected user into a second view for editing
        return View("Manager_EditUserDetails", user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Manager_SaveUserDetails(User updatedUser)
    {
        var user = _context.Users.FirstOrDefault(u => u.EmployeeNumber == updatedUser.EmployeeNumber);
        if (user == null)
        {
            return NotFound();
        }

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.Role = updatedUser.Role;
        user.Name = updatedUser.Name;
        user.Surname = updatedUser.Surname;

        _context.SaveChanges();
        return RedirectToAction("Manager_ChooseUserToEdit");
    }


}