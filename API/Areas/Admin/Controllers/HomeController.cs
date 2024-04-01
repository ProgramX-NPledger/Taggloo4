using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admin;

[Authorize(AuthenticationSchemes = "Identity.Application")]
[Area("Admin")]
[Authorize(Roles = "administrator")]
public class HomeController : Controller
{
    
    public IActionResult Index()
    {
        return View();
    }
    
}