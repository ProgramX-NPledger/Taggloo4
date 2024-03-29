using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}