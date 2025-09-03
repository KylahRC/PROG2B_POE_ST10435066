using Microsoft.AspNetCore.Mvc;

public class ManagerController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
