using Microsoft.AspNetCore.Mvc;

public class CoordinatorController : Controller
{
    public IActionResult Dashboard()
    {
        return View(); // Coordinator dashboard
    }

    public IActionResult PendingClaims()
    {
        return View(); // View pending claims
    }

    public IActionResult ApprovedClaims()
    {
        return View(); // View approved claims
    }

    public IActionResult DeniedClaims()
    {
        return View(); // View denied claims
    }

    [HttpPost]
    public IActionResult ApproveClaim(int id)
    {
        // Simulate approval — no backend logic
        return RedirectToAction("PendingClaims");
    }

    [HttpPost]
    public IActionResult DenyClaim(int id)
    {
        // Simulate denial — no backend logic
        return RedirectToAction("PendingClaims");
    }

    public IActionResult ReviewClaim(int id)
    {
        ViewBag.ClaimId = id; // Optional: pass ID for display only
        return View(); // Review claim details
    }
}
