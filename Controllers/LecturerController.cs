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
        return View(); // Displays lecturer dashboard
    }

    public IActionResult Lecturer_SubmitClaim()
    {
        return View(); // Displays claim submission form
    }

    public IActionResult Lecturer_ViewClaims()
    {
        return View(); // Should match the name of your .cshtml file
    }

    [HttpPost]
    public IActionResult SubmitClaim(string claimMonth, string claimType, decimal hoursWorked, decimal hourlyRate, string notes)
    {
        Console.WriteLine("SubmitClaim action triggered");

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
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Submission failed: " + ex.Message;
        }

        Console.WriteLine($"Received: {claimMonth}, {claimType}, {hoursWorked}, {hourlyRate}, {notes}");
        Console.WriteLine($"Saving claim for {empNum}: {claimMonth}, {claimType}, {hoursWorked}, {hourlyRate}");


        TempData["Success"] = "Claim submitted successfully!";
        return RedirectToAction("Lecturer_Dashboard");

    }


}
