using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleWpfToMvcApp.Web.Models;
using SimpleWpfToMvcApp.Web.Repositories;

namespace SimpleWpfToMvcApp.Web.Controllers;

[Authorize]
public class UsersController(
    IUserRepository userRepository,
    ILogger<UsersController> logger) : Controller
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<UsersController> _logger = logger;

    // GET: Users
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Users index page accessed by user: {User}", User.Identity?.Name);
        var users = await _userRepository.GetAllUsersAsync();
        return View(users);
    }

    // GET: Users/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return NotFound();
        }

        return View(user);
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        return View(new User { Name = string.Empty, Email = string.Empty });
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        // Check for duplicate email
        if (await _userRepository.EmailExistsAsync(user.Email))
        {
            ModelState.AddModelError(nameof(user.Email), "Email already exists. Please enter a unique email address.");
            return View(user);
        }

        try
        {
            await _userRepository.AddUserAsync(user);
            _logger.LogInformation("User created successfully: {UserName} by admin: {Admin}", 
                user.Name, User.Identity?.Name);
            
            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {UserName}", user.Name);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the user. Please try again.");
            return View(user);
        }
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(user);
        }

        // Check for duplicate email (excluding current user)
        if (await _userRepository.EmailExistsAsync(user.Email, user.Id))
        {
            ModelState.AddModelError(nameof(user.Email), "Email already exists. Please enter a unique email address.");
            return View(user);
        }

        try
        {
            await _userRepository.UpdateUserAsync(user);
            _logger.LogInformation("User updated successfully: {UserName} by admin: {Admin}", 
                user.Name, User.Identity?.Name);
            
            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserName}", user.Name);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the user. Please try again.");
            return View(user);
        }
    }

    // POST: Users/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteUserAsync(id);
            _logger.LogInformation("User deleted successfully: {UserName} by admin: {Admin}", 
                user.Name, User.Identity?.Name);
            
            TempData["SuccessMessage"] = "User deleted successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the user. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}
