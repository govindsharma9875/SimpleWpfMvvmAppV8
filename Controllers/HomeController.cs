using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleWpfToMvcApp.Web.Controllers;

[Authorize]
public class HomeController(ILogger<HomeController> logger) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed by user: {User}", User.Identity?.Name);
        return View();
    }

    public IActionResult About()
    {
        _logger.LogInformation("About page accessed by user: {User}", User.Identity?.Name);
        
        var aboutInfo = new
        {
            ApplicationName = "Simple WPF to MVC Migration",
            Version = "1.0.0",
            Description = "A demonstration of migrating WPF MVVM application to ASP.NET Core MVC with modern patterns.",
            MigrationDate = DateTime.Now.ToString("MMMM dd, yyyy"),
            Technologies = new[] { ".NET 8", "ASP.NET Core MVC", "Entity Framework Core", "Bootstrap 5" }
        };

        return View(aboutInfo);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
