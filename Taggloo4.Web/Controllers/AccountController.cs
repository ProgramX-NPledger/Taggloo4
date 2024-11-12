using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Data.EntityFrameworkCore.Identity;
using Taggloo4.Web.Model;
using Taggloo4.Web.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Taggloo4.Web.Controllers;

/// <summary>
/// Controller for login/log out and user registration functionality.
/// </summary>
[Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},Identity.Application")]
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    /// <summary>
    /// Constructor with injected parameters for access to the ASP.NET Identity Taggloo4.Web.
    /// </summary>
    /// <param name="userManager">Implementation of <see cref="UserManager{AppUser}"/>.</param>
    /// <param name="signInManager">Implementation of <see cref="SignInManager{AppUser}"/>.</param>
    public AccountController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    /// <summary>
    /// Returns the Login page.
    /// </summary>
    /// <returns>Login view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        LoginViewModel model = new LoginViewModel()
        {
            Password = string.Empty,
            EmailOrUserName = User.Identity?.Name ?? string.Empty,
            RememberMe = false,
            LogoutSuccessful = Request.Query["m"] == "2"
        };
        return View(model);
    }

    /// <summary>
    /// Accepts Login submission requests.
    /// </summary>
    /// <param name="loginViewModel">View model representing login request.</param>
    /// <returns>If authentication is successful, redirects the user to the home page with <c>m=1</c>, otherwise recycles the current page to
    /// display validation error messages.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (ModelState.IsValid)
        {
            string upperedUserName = loginViewModel.EmailOrUserName.ToUpper();
            AppUser? appUser = await _userManager.Users.SingleOrDefaultAsync(q => q.NormalizedUserName == upperedUserName);
            if (appUser == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }
            else
            {
                var signInResult = await _signInManager.PasswordSignInAsync(appUser, loginViewModel.Password, loginViewModel.RememberMe, false);
                if (signInResult.Succeeded)
                {
                    if (Request.Query.ContainsKey("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"]!);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new
                        {
                            m = 1
                        });
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt");                    
            }

            
        }
        return View(loginViewModel);
    }
    
    /// <summary>
    /// Returns the registration page.
    /// </summary>
    /// <returns>The <c>Register</c> view.</returns>
    public IActionResult Register()
    {
        return View(new RegisterViewModel()
        {
            Email = string.Empty,
            Password = string.Empty,
            ConfirmPassword = string.Empty
        });
    }

    /// <summary>
    /// Accepts user registration requests.
    /// </summary>
    /// <param name="model">View model representing the registration request.</param>
    /// <returns>If registration is successful, signs the user in and returns to the home page with <c>m=3</c>, otherwise
    /// recycles the page to display validation message.</returns>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser()
            {
                UserName = model.Email,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("index", "Home", new { m=3 });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

        }
        return View(model);
    }
    
    /// <summary>
    /// Logs the currently logged in user out.
    /// </summary>
    /// <returns>Redirects to the home page with <c>m=2</c>.</returns>
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login",new { m=2 });
    }
    
}