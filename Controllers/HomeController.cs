using Microsoft.AspNetCore.Mvc;

namespace MonthlyClaimsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClaimDbContext _context;

        public HomeController(ClaimDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(); // Home page
        }

        public IActionResult Privacy()
        {
            return View(); // Privacy page
        }

        public IActionResult Login(string role)
        {
            ViewBag.Role = role;
            return View(); // Login form with role context
        }

        [HttpPost]
        public IActionResult Login(string role, string username, string employeeNumber, string email)
        {
            // Normalize input for comparison
            var normalizedUsername = username?.Trim().ToLower();
            var normalizedEmployeeNumber = employeeNumber?.Trim();
            var normalizedEmail = email?.Trim().ToLower();
            var normalizedRole = role?.Trim().ToLower();

            // Query the database for a matching user
            var user = _context.Users.FirstOrDefault(u =>
                u.EmployeeNumber != null && u.EmployeeNumber.Trim() == normalizedEmployeeNumber &&
                u.Username != null && u.Username.Trim().ToLower() == normalizedUsername &&
                u.Email != null && u.Email.Trim().ToLower() == normalizedEmail &&
                u.Role != null && u.Role.Trim().ToLower() == normalizedRole);



            if (user == null)
            {
                TempData["LoginError"] = "User not found. Please check your details.";
                return RedirectToAction("Login", new { role });
            }

            // Optional: store username in session
            HttpContext.Session.SetString("EmployeeNumber", user.EmployeeNumber);

            // Redirect based on role
            return role switch
            {
                "Lecturer" => RedirectToAction("Lecturer_Dashboard", "Lecturer"),
                "Coordinator" => RedirectToAction("Coordinator_Dashboard", "Coordinator"),
                "Manager" => RedirectToAction("Manager_Dashboard", "Manager"),
                _ => RedirectToAction("Index")
            };
        }


        public IActionResult Error()
        {
            return View(); // Generic error view
        }
    }
}
