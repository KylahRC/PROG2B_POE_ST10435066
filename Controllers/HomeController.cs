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
            var normalizedUsername = username?.Trim().ToLower();
            var normalizedEmployeeNumber = employeeNumber?.Trim();
            var normalizedEmail = email?.Trim().ToLower();
            var normalizedRole = role?.Trim().ToLower();

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

            HttpContext.Session.SetString("EmployeeNumber", user.EmployeeNumber);

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
            return View();
        }
    }
}