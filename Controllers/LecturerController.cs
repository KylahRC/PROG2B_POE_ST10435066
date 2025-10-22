using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    .Include(c => c.StatusLogs)
        .ThenInclude(log => log.ChangedByUser)
    .Where(c => c.EmployeeNumber == empNum)
    .OrderByDescending(c => c.SubmittedAt)
    .ToList();

        return View(claims);
    }

    [HttpPost]
    public IActionResult SubmitClaim(Claim claim, IFormFile supportingFile)
    {
        claim.EmployeeNumber = HttpContext.Session.GetString("EmployeeNumber");

        if (supportingFile != null && supportingFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(supportingFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                supportingFile.CopyTo(stream);
            }

            claim.AttachmentPath = $"/uploads/{fileName}";
            claim.AttachmentName = supportingFile.FileName;
            claim.AttachmentSize = supportingFile.Length;
        }

        claim.SubmittedAt = DateTime.Now;
        claim.Status = "Pending";
        _context.Claims.Add(claim);
        _context.SaveChanges();

        return RedirectToAction("ClaimConfirmation", new { id = claim.ClaimId });
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