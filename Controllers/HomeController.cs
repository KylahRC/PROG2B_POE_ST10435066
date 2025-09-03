using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MonthlyClaimsSystem.Models;

namespace MonthlyClaimsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            TempData["Username"] = username;

            switch (role)
            {
                case "Lecturer":
                    return RedirectToAction("Dashboard", "Lecturer");
                case "Coordinator":
                    return RedirectToAction("Dashboard", "Coordinator");
                case "Manager":
                    return RedirectToAction("Dashboard", "Manager");
                default:
                    return RedirectToAction("Index");
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
