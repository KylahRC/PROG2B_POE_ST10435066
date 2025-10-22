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
                return View();
            }


            // Displays all approved claims with lecturer and status log details.
            public IActionResult Coordinator_ApprovedClaims()
            {
                // Fetch approved claims with related lecturer and status log details
                var approvedClaims = _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.StatusLogs)
                        .ThenInclude(log => log.ChangedByUser)
                    .Where(c => c.Status == "Approved")
                    .OrderByDescending(c => c.SubmittedAt)
                    .ToList();

                return View(approvedClaims);
            }


            // Displays all denied claims with lecturer and status log details.
            public IActionResult Coordinator_DeniedClaims()
            {
                // Fetch denied claims with related lecturer and status log details
                var deniedClaims = _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.StatusLogs)
                        .ThenInclude(log => log.ChangedByUser)
                    .Where(c => c.Status == "Denied")
                    .OrderByDescending(c => c.SubmittedAt)
                    .ToList();

                return View(deniedClaims);
            }


            // Displays all pending claims with lecturer and status log details.
            public IActionResult Coordinator_PendingClaims()
            {
                // Fetch pending claims with related lecturer and status log details
                var pendingClaims = _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.StatusLogs)
                        .ThenInclude(log => log.ChangedByUser)
                    .Where(c => c.Status == "Pending")
                    .OrderByDescending(c => c.SubmittedAt)
                    .ToList();

                return View(pendingClaims);
            }


            // Displays detailed view for reviewing a specific claim.
            public IActionResult Coordinator_ReviewClaim(int id)
            {
                // Fetch the claim along with lecturer and status logs
                var claim = _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.StatusLogs)
                    .FirstOrDefault(c => c.ClaimId == id);

                // Determine if this is the first review based on the presence of status logs
                bool isFirstReview = claim.StatusLogs == null || !claim.StatusLogs.Any();
                ViewBag.IsFirstReview = isFirstReview;

                // If claim not found, redirect with error
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("Coordinator_PendingClaims");
                }

                return View(claim);
            }

    #endregion Dashboard and Claim Views

        #region Action Methods for Approving/Denying/Overriding Claims

            // Approves a claim and logs the status change (no reason needed for approving).
            [HttpPost]
                public IActionResult ApproveClaim(int id)
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


            // Denies a claim with a reason and logs the status change.
            [HttpPost]
                public IActionResult DenyClaim(int id, string reason)
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


            // Overrides the status of a claim with a new status and reason, logging the change (so one really mean coord can't app/deny everything and ruin it).
            [HttpPost]
                public IActionResult OverrideClaimStatus(int id, string newStatus, string reason)
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

        #endregion Action Methods for Approving/Denying/Overriding Claims

    #endregion Public Methods
}