using Microsoft.AspNetCore.Mvc;

public class LecturerController : Controller
{
    public IActionResult Dashboard()
    {
        return View(); // Displays lecturer dashboard
    }

    public IActionResult SubmitClaim()
    {
        return View(); // Displays claim submission form
    }

    public IActionResult ViewClaims()
    {
        return View(); // Should match the name of your .cshtml file
    }

}
