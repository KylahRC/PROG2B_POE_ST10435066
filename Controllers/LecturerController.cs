using Microsoft.AspNetCore.Mvc;
using MonthlyClaimsSystem.Models; // Or whatever namespace contains Claim

public class LecturerController : Controller
{
    private readonly ClaimDbContext _context;

    public LecturerController(ClaimDbContext context)
    {
        _context = context;
    }

    public IActionResult Lecturer_Dashboard()
    {
        return View();
    }

    public IActionResult Lecturer_SubmitClaim()
    {
        return View();
    }

    public IActionResult Lecturer_ViewClaims()
    {
        var empNum = HttpContext.Session.GetString("EmployeeNumber");
        if (string.IsNullOrEmpty(empNum))
        {
            TempData["Error"] = "Session expired. Please log in again.";
            return RedirectToAction("Login", "Home");
        }

        var claims = _context.Claims
            .Where(c => c.EmployeeNumber == empNum)
            .OrderByDescending(c => c.SubmittedAt)
            .ToList();

        return View(claims);
    }


    [HttpPost]
    public IActionResult SubmitClaim(string claimMonth, string claimType, decimal hoursWorked, decimal hourlyRate, string notes)
    {
        var empNum = HttpContext.Session.GetString("EmployeeNumber");
        if (string.IsNullOrEmpty(empNum))
        {
            TempData["Error"] = "Session expired. Please log in again.";
            return RedirectToAction("Login", "Home");
        }

        try
        {
            var claim = new Claim
            {
                EmployeeNumber = empNum,
                ClaimMonth = claimMonth,
                ClaimType = claimType,
                HoursWorked = hoursWorked,
                HourlyRate = hourlyRate,
                Notes = notes,
                Status = "Pending",
                SubmittedAt = DateTime.Now
            };

            _context.Claims.Add(claim);
            _context.SaveChanges();

            return RedirectToAction("ClaimConfirmation", new { id = claim.ClaimId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Submission failed: " + ex.Message;
        }

        return RedirectToAction("Lecturer_Dashboard");
    }

    public IActionResult ClaimConfirmation(int id)
    {
        var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
        if (claim == null)
        {
            TempData["Error"] = "Claim not found.";
            return RedirectToAction("Lecturer_Dashboard");
        }

        return View(claim);
    }
}