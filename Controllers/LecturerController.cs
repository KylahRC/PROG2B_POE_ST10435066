using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models; // Or whatever namespace contains Claim

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

            // Displays the claim confirmation view for a submitted claim.
            public IActionResult ClaimConfirmation(int id)
            {
                // Retrieve the claim details using the provided ID
                var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("Lecturer_Dashboard");
                }

                return View(claim);
            }


            // Displays the lecturer dashboard view.
            public IActionResult Lecturer_Dashboard()
            {
                return View();
            }


            // Displays the claim submission form view.
            public IActionResult Lecturer_SubmitClaim()
            {
                return View();
            }


            // Displays all claims submitted by the logged-in lecturer with status logs and user details.
            public IActionResult Lecturer_ViewClaims()
            {
                // Get the logged-in lecturer's employee number from session
                var empNum = HttpContext.Session.GetString("EmployeeNumber");

                // If not found, redirect to login
                if (string.IsNullOrEmpty(empNum))
                {
                    TempData["Error"] = "Session expired. Please log in again.";
                    return RedirectToAction("Login", "Home");
                }

                // Retrieve claims with status logs and user details
                // Order by submission date descending
                var claims = _context.Claims
                    .Include(c => c.StatusLogs)
                        // Include the user who made each status change
                        .ThenInclude(log => log.ChangedByUser)
                    .Where(c => c.EmployeeNumber == empNum)
                    .OrderByDescending(c => c.SubmittedAt)
                    .ToList();

                return View(claims);
            }

    #endregion Dashboard and Claim Views

        #region Action Methods for Submitting Claims

            // Handles the submission of a new claim, including optional file upload.
            [HttpPost]
                public IActionResult SubmitClaim(Claim claim, IFormFile supportingFile)
                {
                    claim.EmployeeNumber = HttpContext.Session.GetString("EmployeeNumber");

                    // Handle file upload if a file is provided
                    if (supportingFile != null && supportingFile.Length > 0)
                    {
                        // Save the file to a designated folder (e.g., "wwwroot/uploads")
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                        Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                        // Generate a unique file name to avoid conflicts
                        // Use a GUID (which stands for Globally Unique Identifier which is a 128-bit integer used to uniquely identify information) combined with the original file name
                        // We did this because if two users upload a file with the same name, one could overwrite the other and thats bad
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(supportingFile.FileName)}";

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            supportingFile.CopyTo(stream);
                        }

                        // Store the file path or name in the claim record
                        claim.AttachmentPath = $"/uploads/{fileName}";
                        claim.AttachmentName = supportingFile.FileName;
                        claim.AttachmentSize = supportingFile.Length;
                    }

                    // Set initial claim properties
                    // These details are captured server-side to ensure integrity, the lecturer can't manipulate these values in the form
                    claim.SubmittedAt = DateTime.Now;
                    claim.Status = "Pending";

                    _context.Claims.Add(claim);

                    _context.SaveChanges();

                    return RedirectToAction("ClaimConfirmation", new { id = claim.ClaimId });
                }

        #endregion Action Methods for Submitting Claims

    #endregion Public Methods
}