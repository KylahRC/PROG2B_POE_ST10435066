using Microsoft.AspNetCore.Mvc;

public class LecturerController : Controller
{
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

}
