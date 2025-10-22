using Microsoft.AspNetCore.Mvc;

public class ManagerController : Controller
{
    public IActionResult Manager_Dashboard()
    {
        return View();
    }
}