using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SimpleWpfToMvcApp.Web.Services;

public interface IAuthenticationService
{
    Task<bool> ValidateCredentialsAsync(string username, string password);
    Task SignInUserAsync(HttpContext context, string username);
    Task SignOutUserAsync(HttpContext context);
    bool IsUserAuthenticated(HttpContext context);
}

public class AuthenticationService(ILogger<AuthenticationService> logger) : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger = logger;
    
    // Simple hardcoded credentials for demo (in real app, use database/identity)
    private const string ValidUsername = "ankit";
    private const string ValidPassword = "password@123";

    public Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var isValid = string.Equals(username, ValidUsername, StringComparison.OrdinalIgnoreCase) 
                     && password == ValidPassword;
        
        _logger.LogInformation("Login attempt for user: {Username}, Success: {IsValid}", username, isValid);
        return Task.FromResult(isValid);
    }

    public async Task SignInUserAsync(HttpContext context, string username)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, username),
            new("DisplayName", username)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
        };

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("User signed in successfully: {Username}", username);
    }

    public async Task SignOutUserAsync(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User signed out successfully");
    }

    public bool IsUserAuthenticated(HttpContext context)
    {
        return context.User?.Identity?.IsAuthenticated == true;
    }
}
