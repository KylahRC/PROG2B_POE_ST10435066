using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;

public class CoordinatorController : Controller
{
    #region Private Fields

    private readonly ClaimDbContext _context;

    #endregion Private Fields

    #region Public Constructors

    public CoordinatorController(ClaimDbContext context)
    {
        _context = context;
    }

    #endregion Public Constructors

    #region Public Methods

        #region Dashboard and Claim Views

            // Renders the coordinator dashboard view.
            public IActionResult Coordinator_Dashboard()
            {
                try
                {
                    // Check session for role and employee number
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");

                    // If not logged in as Coordinator, redirect to error
                    if (string.IsNullOrEmpty(role) || role != "Coordinator" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    // Format username for display
                    // E.g., "JSmith" becomes "Smith"
                    var rawUsername = HttpContext.Session.GetString("Username") ?? "Coordinator";

                    // Default display name if parsing fails
                    string displayName = "Coordinator";

                    // Parse and format the username
                    if (!string.IsNullOrEmpty(rawUsername) && rawUsername.Length > 1)
                    {
                        // Remove the first character (initial)
                        var surnamePart = rawUsername.Substring(1);

                        // Capitalize the first letter of the surname
                        displayName = char.ToUpper(surnamePart[0]) + surnamePart.Substring(1).ToLower();
                    }

                    // Set the formatted name in ViewBag for the view
                    ViewBag.Username = displayName;

                    return View();
                }
                catch (Exception ex)
                {
                    // Handle any unexpected errors
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }

            // Displays all approved claims with lecturer and status log details.
            public IActionResult Coordinator_ApprovedClaims()
            {
                try
                {
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");

                    if (string.IsNullOrEmpty(role) || role != "Coordinator" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    var approvedClaims = _context.Claims
                        .Include(c => c.Lecturer)
                        .Include(c => c.StatusLogs) 
                        .Where(c => c.Status == "Approved")
                        .OrderByDescending(c => c.SubmittedAt)
                        .ToList();

                    return View(approvedClaims);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }

            // Displays all denied claims with lecturer and status log details.
            public IActionResult Coordinator_DeniedClaims()
            {
                try
                {
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");

                    if (string.IsNullOrEmpty(role) || role != "Coordinator" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    var deniedClaims = _context.Claims
                        .Include(c => c.Lecturer)
                        .Include(c => c.StatusLogs) 
                        .Where(c => c.Status == "Denied")
                        .OrderByDescending(c => c.SubmittedAt)
                        .ToList();

                    return View(deniedClaims);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }

            // Displays all pending claims with lecturer and status log details.
            public IActionResult Coordinator_PendingClaims()
            {
                try
                {
                    // Check session for role and employee number
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");


                    // If not logged in as Coordinator, redirect to error
                    if (string.IsNullOrEmpty(role) || role != "Coordinator" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    // Fetch pending claims with lecturer and status logs
                    var pendingClaims = _context.Claims
                        .Include(c => c.Lecturer)
                        .Include(c => c.StatusLogs) 
                        .Where(c => c.Status == "Pending")
                        .OrderByDescending(c => c.SubmittedAt)
                        .ToList();

                    return View(pendingClaims);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }

            // Displays detailed view for reviewing a specific claim.
            public IActionResult Coordinator_ReviewClaim(int id)
            {
                try
                {
                    // Check session for role and employee number
                    var role = HttpContext.Session.GetString("Role");
                    var empNum = HttpContext.Session.GetString("EmployeeNumber");

                    // If not logged in as Coordinator, redirect to error
                    if (string.IsNullOrEmpty(role) || role != "Coordinator" || string.IsNullOrEmpty(empNum))
                    {
                        TempData["ErrorMessage"] = "Access denied. Please log in first.";
                        return RedirectToAction("Error", "Home", new { code = 403 });
                    }

                    // Fetch the claim along with lecturer and status logs
                    var claim = _context.Claims
                        .Include(c => c.Lecturer)
                        .Include(c => c.StatusLogs)
                        .FirstOrDefault(c => c.ClaimId == id);

                    if (claim == null)
                    {
                        TempData["Error"] = "Claim not found.";
                        return RedirectToAction("Coordinator_PendingClaims");
                    }

                    //// Determine if this is the first review
                    //bool isFirstReview = claim.StatusLogs == null || !claim.StatusLogs.Any();
                    //ViewBag.IsFirstReview = isFirstReview;

                    // Check current status
                    bool isPending = claim.Status == "Pending";
                    ViewBag.IsPending = isPending;

                    // If not pending, then it's already approved/denied ? only override allowed
                    bool isFinalised = claim.Status == "Approved" || claim.Status == "Denied";
                    ViewBag.IsFinalised = isFinalised;

                    return View(claim);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                    return RedirectToAction("Error", "Home");
                }
            }

            // Logs out the user by clearing cache and redirecting to home.
            // This helps prevent back button access after logout.
            public IActionResult Logout()
            {
                // Clear the session data for security reasons
                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";
                return RedirectToAction("Index", "Home"); // Or redirect to login
            }

        #endregion Dashboard and Claim Views

        #region Action Methods for Approving/Denying/Overriding Claims

            // Approves a claim and logs the status change (no reason needed for approving).
            [HttpPost]
                public IActionResult ApproveClaim(int id)
                {
                    try
                    { 
                        // Find the claim by ID
                        var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);

                        // If found, update status and log the change
                        if (claim != null)
                        {
                            claim.Status = "Approved";

                            // Log the status change
                            _context.ClaimStatusLogs.Add(new ClaimStatusLog
                            {
                                ClaimId = id,
                                ChangedBy = HttpContext.Session.GetString("EmployeeNumber"),
                                NewStatus = "Approved",
                                ChangeDate = DateTime.Now,
                                Reason = null
                            });

                        // Save changes to the database
                        _context.SaveChanges();
                        }
                        return RedirectToAction("Coordinator_PendingClaims");

                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                        return RedirectToAction("Error", "Home");
                    }
                }


            // Denies a claim with a reason and logs the status change.
            [HttpPost]
                public IActionResult DenyClaim(int id, string reason)
                {
                    try
                    { 
                        // Find the claim by ID
                        var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);

                        // If found, update status and log the change
                        if (claim != null)
                        {
                            claim.Status = "Denied";

                            // Log the status change with reason
                            _context.ClaimStatusLogs.Add(new ClaimStatusLog
                            {
                                ClaimId = id,
                                ChangedBy = HttpContext.Session.GetString("EmployeeNumber"),
                                NewStatus = "Denied",
                                ChangeDate = DateTime.Now,
                                Reason = reason
                            });

                            // Save changes to the database
                            _context.SaveChanges();
                        }
                        return RedirectToAction("Coordinator_PendingClaims");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                        return RedirectToAction("Error", "Home");
                    }
                }


            // Overrides the status of a claim with a new status and reason, logging the change (so one really mean coord can't app/deny everything and ruin it).
            [HttpPost]
                public IActionResult OverrideClaimStatus(int id, string newStatus, string reason)
                {
                    try
                    { 
                        // Find the claim by ID
                        var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);

                        // If found, update status and log the change
                        if (claim != null)
                        {
                            claim.Status = newStatus;

                            // Log the status change with reason
                            _context.ClaimStatusLogs.Add(new ClaimStatusLog
                            {
                                ClaimId = id,
                                ChangedBy = HttpContext.Session.GetString("EmployeeNumber"),
                                NewStatus = newStatus,
                                ChangeDate = DateTime.Now,
                                Reason = reason
                            });

                            // Save changes to the database
                            _context.SaveChanges();
                        }

                        return RedirectToAction("Coordinator_ReviewClaim", new { id });
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Something went wrong: {ex.Message}";
                        return RedirectToAction("Error", "Home");
                    }
                }

        #endregion Action Methods for Approving/Denying/Overriding Claims

    #endregion Public Methods
}