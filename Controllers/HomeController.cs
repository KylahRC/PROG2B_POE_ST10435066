using Microsoft.AspNetCore.Mvc;

namespace MonthlyClaimsSystem.Controllers
{
    public class HomeController : Controller
    {
        #region Private Fields

        private readonly ClaimDbContext _context;

        #endregion Private Fields

        #region Public Constructors

        public HomeController(ClaimDbContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

            #region Standard Methods

                // GET: /Home/Error
                // Renders the error view.
                public IActionResult Error()
                {
                    return View();
                }


                // GET: /Home/Index
                // Renders the home page view.
                public IActionResult Index()
                {
                    return View();
                }


                // GET: /Home/Login
                // Renders the login view for the specified role.
                public IActionResult Login(string role)
                {
                    // Pass the role to the view for context
                    ViewBag.Role = role;
                    return View();
                }


                // GET: /Home/Privacy
                // Renders the privacy policy view.
                public IActionResult Privacy()
                {
                    return View();
                }

            #endregion Standard Methods

            #region Login Method

                // POST: /Home/Login
                // Authenticates user and redirects to role-specific dashboard.
                [HttpPost]
                    public IActionResult Login(string role, string username, string employeeNumber, string email)
                    {
                        // Normalize inputs for comparison
                        var normalizedUsername = username?.Trim().ToLower();
                        var normalizedEmployeeNumber = employeeNumber?.Trim();
                        var normalizedEmail = email?.Trim().ToLower();
                        var normalizedRole = role?.Trim().ToLower();

                        // Find user matching all criteria
                        // Using FirstOrDefault to avoid exceptions if no match is found            
                        var user = _context.Users.FirstOrDefault(u =>
                            u.EmployeeNumber != null && u.EmployeeNumber.Trim() == normalizedEmployeeNumber &&
                            u.Username != null && u.Username.Trim().ToLower() == normalizedUsername &&
                            u.Email != null && u.Email.Trim().ToLower() == normalizedEmail &&
                            u.Role != null && u.Role.Trim().ToLower() == normalizedRole);

                        // If no user found, redirect back to login with error
                        if (user == null)
                        {
                            TempData["LoginError"] = "User not found. Please check your details.";
                            return RedirectToAction("Login", new { role });
                        }

                        // Store user details in session
                        HttpContext.Session.SetString("EmployeeNumber", user.EmployeeNumber);
                        HttpContext.Session.SetString("Role", user.Role);

                        // Redirect to role-specific dashboard
                        return role switch
                        {
                            "Lecturer" => RedirectToAction("Lecturer_Dashboard", "Lecturer"),
                            "Coordinator" => RedirectToAction("Coordinator_Dashboard", "Coordinator"),
                            "Manager" => RedirectToAction("Manager_Dashboard", "Manager"),
                            _ => RedirectToAction("Index")
                        };
                    }

            #endregion Login Method

        #endregion Public Methods
    }
}