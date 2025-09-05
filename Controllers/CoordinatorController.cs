using Microsoft.AspNetCore.Mvc;

public class CoordinatorController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult PendingClaims()
    {
        return View();
    }

    public IActionResult ApprovedClaims()
    {
        return View();
    }

    public IActionResult DeniedClaims()
    {
        return View();
    }

    

    [HttpPost]
    public IActionResult ApproveClaim(int id)
    {
        // Update claim status to Approved (mocked for now)
        return RedirectToAction("PendingClaims");
    }

    [HttpPost]
    public IActionResult DenyClaim(int id)
    {
        // Update claim status to Denied (mocked for now)
        return RedirectToAction("PendingClaims");
    }

    public IActionResult ReviewClaim(int id)
    {
        ViewBag.ClaimId = id;
        return View();
    }


}
