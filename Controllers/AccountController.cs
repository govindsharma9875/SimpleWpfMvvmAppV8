using Microsoft.AspNetCore.Mvc;
using SimpleWpfToMvcApp.Web.Models;
using SimpleWpfToMvcApp.Web.Services;

namespace SimpleWpfToMvcApp.Web.Controllers;

public class AccountController(
    IAuthenticationService authService,
    ILogger<AccountController> logger) : Controller
{
    private readonly IAuthenticationService _authService = authService;
    private readonly ILogger<AccountController> _logger = logger;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (_authService.IsUserAuthenticated(HttpContext))
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var isValid = await _authService.ValidateCredentialsAsync(model.Username, model.Password);
        if (!isValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        await _authService.SignInUserAsync(HttpContext, model.Username);
        _logger.LogInformation("User {Username} logged in successfully", model.Username);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutUserAsync(HttpContext);
        _logger.LogInformation("User logged out successfully");
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
