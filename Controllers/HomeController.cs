using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MonthlyClaimsSystem.Data;
using MonthlyClaimsSystem.Models;
using Microsoft.AspNetCore.Http;


namespace MonthlyClaimsSystem.Controllers
{

    public class HomeController : Controller
    {

        


        private readonly ILogger<HomeController> _logger;
        private readonly ClaimDbContext _context;

        public HomeController(ILogger<HomeController> logger, ClaimDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login(string role)
        {
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string role, string username, string employeeNumber, string email)
        {


            var user = _context.Users.FirstOrDefault(u =>
            u.Username.ToLower().Trim() == username.ToLower().Trim() &&
            u.EmployeeNumber.ToLower().Trim() == employeeNumber.ToLower().Trim() &&
            u.Email.ToLower().Trim() == email.ToLower().Trim() &&
            u.Role.ToLower().Trim() == role.ToLower().Trim());


            TempData["Debug"] = $"Trying: {username}, {employeeNumber}, {email}, {role}";
            var allUsers = _context.Users.ToList();
            TempData["Debug"] = $"Found {allUsers.Count} users.";



            if (user == null)
            {
                // Optionally show an error or redirect back to login
                TempData["LoginError"] = "User not found. Please check your details.";
                return RedirectToAction("Login", new { role });
            }

            HttpContext.Session.SetString("Username", user.Username);

            return role switch
            {
                "Lecturer" => RedirectToAction("Dashboard", "Lecturer"),
                "Coordinator" => RedirectToAction("Dashboard", "Coordinator"),
                "Manager" => RedirectToAction("Dashboard", "Manager"),
                _ => RedirectToAction("Index")
            };
        }

        



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
