using Microsoft.AspNetCore.Mvc;

public class LecturerController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
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
        return View();
    }

   

}

