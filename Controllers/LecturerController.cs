using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models; 

public class LecturerController : Controller
{
    #region Private Fields

        private readonly ClaimDbContext _context;

        
    #endregion Private Fields

    #region Public Constructors

        public LecturerController(ClaimDbContext context)
        {
            _context = context;
        }

    #endregion Public Constructors

    #region Public Methods


        #region Dashboard and Claim Views

            // Displays the lecturer dashboard view.
            public IActionResult Lecturer_Dashboard()
            {
                try
                {
                    // Retrieve session data
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");

                    // Get the raw username from session
                    var rawUsername = HttpContext.Session.GetString("Username") ?? "Lecturer";

                    // Access control: Ensure user is logged in as Lecturer
                    if (string.IsNullOrEmpty(role) || role != "Lecturer" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    // Format the display name by removing the initial and capitalizing the surname
                    string displayName = "Lecturer";
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
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }


            // Displays the claim submission form view.
            public IActionResult Lecturer_SubmitClaim()
            {
                try
                {
                    // Retrieve session data
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");

                    // Access control: Ensure user is logged in as Lecturer
                    if (string.IsNullOrEmpty(role) || role != "Lecturer" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    return View();
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }


            // Displays the claim confirmation view for a submitted claim.
            public IActionResult ClaimConfirmation(int id)
            {
                try
                {
                    // Retrieve session data
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");

                    // Access control: Ensure user is logged in as Lecturer
                    if (string.IsNullOrEmpty(role) || role != "Lecturer" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    // Retrieve the claim details using the provided ID
                    var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
                    if (claim == null)
                    {
                        TempData["Error"] = "Claim not found.";
                        return RedirectToAction("Lecturer_Dashboard");
                    }

                    return View(claim);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }


            // Displays all claims submitted by the logged-in lecturer with status logs
            public IActionResult Lecturer_ViewClaims()
            {
                try
                {
                    // Retrieve session data
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");

     
                    // Current month name (e.g., "November")
                    var currentMonthName = DateTime.Now.ToString("MMMM");

                    // Totals for the current month, scoped to this lecturer
                    var approvedTotalForCurrentMonth = _context.Claims
                        .Where(c => c.Status == "Approved"
                                 && c.ClaimMonth == currentMonthName
                                 && c.EmployeeNumber == empNum)
                        .Sum(c => c.HoursWorked * c.HourlyRate);

                    // Pass totals to the view using ViewBag
                    ViewBag.ApprovedTotal = approvedTotalForCurrentMonth;

                    // Access control: Ensure user is logged in as Lecturer
                    if (string.IsNullOrEmpty(role) || role != "Lecturer" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    // If not found, redirect to login
                    if (string.IsNullOrEmpty(empNum))
                    {
                        TempData["Error"] = "Session expired. Please log in again.";
                        return RedirectToAction("Login", "Home");
                    }

            // Retrieve claims with status logs
            var claims = _context.Claims
                .Include(c => c.StatusLogs)
                .Where(c => c.EmployeeNumber == empNum)
                .OrderByDescending(c => c.SubmittedAt)
                .ToList();

            // Enrich logs with coordinator names
            foreach (var claim in claims)
            {
                foreach (var log in claim.StatusLogs)
                {
                    var user = _context.Users
                        .FirstOrDefault(u => u.EmployeeNumber == log.ChangedBy);

                    if (user != null)
                    {
                        log.ChangedBy = $"{user.Name} {user.Surname}";
                    }
                }
            }


            return View(claims);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }


        #endregion Dashboard and Claim Views

        #region Action Methods for Submitting Claims

            // Handles the submission of a new claim, requiring a file upload.
            [HttpPost]
            public IActionResult SubmitClaim(Claim claim, IFormFile supportingFile)
            {
                try
                {
                    claim.EmployeeNumber = HttpContext.Session.GetString("EmployeeNumber");

                    // Enforce required file upload
                    if (supportingFile == null || supportingFile.Length == 0)
                    {
                        ModelState.AddModelError("Attachment", "Supporting document is required.");
                        return View("Lecturer_SubmitClaim", claim);
                        // Return to the form with validation error
                    }

                    // Save the file to a designated folder (wwwroot/uploads)
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                    // Generate a unique file name to avoid conflicts
                    var originalName = Path.GetFileNameWithoutExtension(supportingFile.FileName);
                    var extension = Path.GetExtension(supportingFile.FileName);
                    var fileName = $"{originalName}_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        supportingFile.CopyTo(stream);
                    }

                    // Store the file path or name in the claim record
                    claim.AttachmentPath = $"/uploads/{fileName}";
                    claim.AttachmentName = supportingFile.FileName;
                    claim.AttachmentSize = supportingFile.Length;

                    // Set initial claim properties
                    claim.SubmittedAt = DateTime.Now;

                    // Define unpaid categories
                    var unpaidCategories = new[] { "Invigilation", "Meeting", "Training", "Outreach", "Admin" };

                    // Determine claim status based on type and hourly rate
                    if (unpaidCategories.Contains(claim.ClaimType))
                    {
                        if (claim.HourlyRate == 0)
                        {
                            claim.Status = "Approved";
                        }
                        else
                        {
                            claim.Status = "Denied";
                        }
                    }
                    else
                    {
                        claim.Status = "Pending";
                    }

                    // Save claim first
                    _context.Claims.Add(claim);
                    _context.SaveChanges();

                    // Add system log entry
                    var log = new ClaimStatusLog
                    {
                        ClaimId = claim.ClaimId,
                        ChangedBy = "System",
                        NewStatus = claim.Status,
                        ChangeDate = DateTime.Now,
                        Reason = claim.Status switch
                        {
                            "Approved" => "Auto-approved by system",
                            "Denied" => "Auto-denied by system due to hourly rate discrepancies",
                            "Pending" => "Paid work requires manual review",
                            _ => "Something went very wrong, check system"
                        },
                        IsSystemChange = true
                    };

                    // Save log entry
                    _context.ClaimStatusLogs.Add(log);
                    _context.SaveChanges();

                    // Redirect to confirmation page
                    return RedirectToAction("ClaimConfirmation", new { id = claim.ClaimId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }

            // Logs out the user by clearing the session and redirects to home.
            public IActionResult Logout()
            {
                HttpContext.Session.Clear(); // Wipes all session data
                return RedirectToAction("Index", "Home"); // Or redirect to login
            }


        #endregion Action Methods for Submitting Claims

    #endregion Public Methods
}