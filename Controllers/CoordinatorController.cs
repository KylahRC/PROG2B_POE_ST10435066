using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Models;

public class CoordinatorController : Controller
{
    private readonly ClaimDbContext _context;

    public CoordinatorController(ClaimDbContext context)
    {
        _context = context;
    }

    public IActionResult Coordinator_Dashboard()
    {
        return View();
    }

    public IActionResult Coordinator_PendingClaims()
    {
        var pendingClaims = _context.Claims
            .Where(c => c.Status == "Pending")
            .OrderByDescending(c => c.SubmittedAt)
            .ToList();

        return View(pendingClaims);
    }

    public IActionResult Coordinator_ApprovedClaims()
    {
        var approvedClaims = _context.Claims
            .Include(c => c.Lecturer)
            .Where(c => c.Status == "Approved")
            .OrderByDescending(c => c.SubmittedAt)
            .ToList();

        return View(approvedClaims);
    }

    public IActionResult Coordinator_DeniedClaims()
    {
        var deniedClaims = _context.Claims
            .Include(c => c.Lecturer)
            .Where(c => c.Status == "Denied")
            .OrderByDescending(c => c.SubmittedAt)
            .ToList();

        return View(deniedClaims);
    }

    public IActionResult Coordinator_ReviewClaim(int id)
    {
        var claim = _context.Claims
            .Include(c => c.Lecturer)
            .Include(c => c.StatusLogs)
            .FirstOrDefault(c => c.ClaimId == id);

        bool isFirstReview = claim.StatusLogs == null || !claim.StatusLogs.Any();
        ViewBag.IsFirstReview = isFirstReview;

        if (claim == null)
        {
            TempData["Error"] = "Claim not found.";
            return RedirectToAction("Coordinator_PendingClaims");
        }

        return View(claim);
    }

    [HttpPost]
    public IActionResult ApproveClaim(int id)
    {
        var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
        if (claim != null)
        {
            claim.Status = "Approved";
            _context.ClaimStatusLogs.Add(new ClaimStatusLog
            {
                ClaimId = id,
                ChangedBy = HttpContext.Session.GetString("EmployeeNumber"),
                NewStatus = "Approved",
                ChangeDate = DateTime.Now,
                Reason = null
            });

            _context.SaveChanges();
        }
        return RedirectToAction("Coordinator_PendingClaims");
    }

    [HttpPost]
    public IActionResult DenyClaim(int id, string reason)
    {
        var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
        if (claim != null)
        {
            claim.Status = "Denied";
            _context.ClaimStatusLogs.Add(new ClaimStatusLog
            {
                ClaimId = id,
                ChangedBy = HttpContext.Session.GetString("EmployeeNumber"),
                NewStatus = "Denied",
                ChangeDate = DateTime.Now,
                Reason = reason
            });

            _context.SaveChanges();
        }
        return RedirectToAction("Coordinator_PendingClaims");
    }

    [HttpPost]
    public IActionResult OverrideClaimStatus(int id, string newStatus, string reason)
    {
        var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
        if (claim != null)
        {
            claim.Status = newStatus;
            _context.ClaimStatusLogs.Add(new ClaimStatusLog
            {
                ClaimId = id,
                ChangedBy = HttpContext.Session.GetString("EmployeeNumber"),
                NewStatus = newStatus,
                ChangeDate = DateTime.Now,
                Reason = reason
            });
            _context.SaveChanges();
        }

        return RedirectToAction("Coordinator_ReviewClaim", new { id });
    }
}