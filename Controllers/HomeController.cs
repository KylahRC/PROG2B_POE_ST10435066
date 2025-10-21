using Microsoft.AspNetCore.Mvc;

namespace MonthlyClaimsSystem.Controllers
{
    public class HomeController : Controller
    {
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
            // Simulated login — no validation, just redirect based on role
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
