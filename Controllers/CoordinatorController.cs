using Microsoft.AspNetCore.Mvc;

public class CoordinatorController : Controller
{
    public IActionResult Coordinator_Dashboard()
    {
        return View();
    }

    public IActionResult Coordinator_PendingClaims()
    {
        return View();
    }

    public IActionResult Coordinator_ApprovedClaims()
    {
        return View();
    }

    public IActionResult Coordinator_DeniedClaims()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ApproveClaim(int id)
    {
        return RedirectToAction("PendingClaims");
    }

    [HttpPost]
    public IActionResult DenyClaim(int id)
    {
        return RedirectToAction("PendingClaims");
    }

    public IActionResult ReviewClaim(int id)
    {
        ViewBag.ClaimId = id;
        return View();
    }
}