using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;
using MonthlyClaimsSystem.ViewModels;

public class ManagerController : Controller
{
    #region Private Fields

        private readonly ClaimDbContext _context;

    #endregion Private Fields

    #region Public Constructors

        public ManagerController(ClaimDbContext context)
        {
            _context = context;
        }

    #endregion Public Constructors

    #region Public Methods

        #region Standard Methods
            public IActionResult Manager_Dashboard()
            {
                // Retrieve the raw username from session
                var rawUsername = HttpContext.Session.GetString("Username") ?? "Manager";

                // Process the username to remove initial and format it
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

            // GET: Manager/Report
            // Renders the manager report view with filtering options and totals.
            public IActionResult Manager_Report(string[] status, string[] employeeNumber, string[] claimMonth, string[] claimType)
            {
                // Base query for claims including lecturer details
                var claimsQuery = _context.Claims.Include(c => c.Lecturer).AsQueryable();


                // Applies filter for status
                if (status != null && status.Any())
                    claimsQuery = claimsQuery.Where(c => status.Contains(c.Status));

                // Applies filter for employee number
                if (employeeNumber != null && employeeNumber.Any())
                    claimsQuery = claimsQuery.Where(c => employeeNumber.Contains(c.EmployeeNumber));

                // Applies filter for claim month
                if (claimMonth != null && claimMonth.Any())
                    claimsQuery = claimsQuery.Where(c => claimMonth.Contains(c.ClaimMonth));

                // Applies filter for claim type
                if (claimType != null && claimType.Any())
                    claimsQuery = claimsQuery.Where(c => claimType.Contains(c.ClaimType));


                var claims = claimsQuery.ToList();

                // Totals calculation to display on the report just because I thought it would look cool
                // Update: it does
                var approvedTotal = _context.Claims.Where(c => c.Status == "Approved").Sum(c => c.HoursWorked * c.HourlyRate);
                var deniedTotal = _context.Claims.Where(c => c.Status == "Denied").Sum(c => c.HoursWorked * c.HourlyRate);
                var pendingTotal = _context.Claims.Where(c => c.Status == "Pending").Sum(c => c.HoursWorked * c.HourlyRate);

                // Get lecturers list
                var lecturers = _context.Users.Where(u => u.Role == "Lecturer").ToList();

                // Pass data to the view using ViewBag
                ViewBag.Lecturers = lecturers;
                ViewBag.ApprovedTotal = approvedTotal;
                ViewBag.DeniedTotal = deniedTotal;
                ViewBag.PendingTotal = pendingTotal;
                ViewBag.Claims = claims;

                return View();
            }

            // GET: Manager/GenerateInvoices
            // Renders the invoice generation view for approved claims.
            public IActionResult Manager_GenerateInvoices()
            {
                // Gets the current month name
                var currentMonthName = DateTime.Now.ToString("MMMM"); // e.g. "November"

                // Totals for different statuses and timeframes
                var approvedTotalForCurrentMonth = _context.Claims
                    .Where(c => c.Status == "Approved" && c.ClaimMonth == currentMonthName)
                    .Sum(c => c.HoursWorked * c.HourlyRate);

                var approvedTotalForYear = _context.Claims
                    .Where(c => c.Status == "Approved" && c.ClaimMonth != currentMonthName)
                    .Sum(c => c.HoursWorked * c.HourlyRate);

                var pendingTotalForCurrentMonth = _context.Claims
                    .Where(c => c.Status == "Pending" && c.ClaimMonth == currentMonthName)
                    .Sum(c => c.HoursWorked * c.HourlyRate);

                // Pass totals to the view using ViewBag
                ViewBag.ApprovedTotal = approvedTotalForCurrentMonth;
                ViewBag.DeniedTotal = approvedTotalForYear;
                ViewBag.PendingTotal = pendingTotalForCurrentMonth;

                // Group approved claims by EmployeeNumber and ClaimMonth to create invoices
                var approvedClaims = _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == "Approved")
                    .ToList();

        // Create invoices from approved claims
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

        #endregion Standard Methods

        #region Edit User Details

            // POST: Manager/EditPersonalDetails
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Manager_ChooseUserToEdit(string employeeNumber)
            {
                // Retrieve the selected user based on EmployeeNumber
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
                // Retrieve the user from the database
                var user = _context.Users.FirstOrDefault(u => u.EmployeeNumber == updatedUser.EmployeeNumber);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user details
                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;
                user.Name = updatedUser.Name;
                user.Surname = updatedUser.Surname;

                // Save changes to the database
                _context.SaveChanges();
                return RedirectToAction("Manager_ChooseUserToEdit");
            }

        #endregion Edit User Details

    #endregion Public Methods


}