using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonthlyClaimsSystem.Controllers;
using MonthlyClaimsSystem.Data;

public class LecturerController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ClaimDbContext _context;

    public LecturerController(ILogger<HomeController> logger, ClaimDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult SubmitClaim()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SubmitClaim(IFormCollection form)
    {
        // You can access form["month"], form["claimType"], etc. later if needed
        return RedirectToAction("Dashboard");
    }

    public IActionResult ReviewClaimLecturer(int id)
    {
        ViewBag.ClaimId = id;
        
        return View();
    }

    public IActionResult ViewClaims()
    {
        var employeeNumber = HttpContext.Session.GetString("Username"); // now using EmployeeNumber as the key

        var user = _context.Users.FirstOrDefault(u => u.Username == employeeNumber);
        if (user == null || user.Role != "Lecturer")
        {
            return RedirectToAction("Index", "Home");
        }

        var claims = _context.Claims
            .Include(c => c.User) // make sure this is included so you can access Name/Surname
            .Where(c => c.EmployeeNumber == user.EmployeeNumber)
            .OrderByDescending(c => c.SubmittedAt)
            .ToList();

        ViewBag.FullName = $"{user.Name} {user.Surname}";
        return View(claims);
    }



    public IActionResult Dashboard()
    {
        ViewBag.Username = TempData["Username"];
        return View();
    }


    

   

}

