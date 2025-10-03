using Microsoft.EntityFrameworkCore;
using SimpleWpfToMvcApp.Web.Data;
using SimpleWpfToMvcApp.Web.Models;

namespace SimpleWpfToMvcApp.Web.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> AddUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email && (excludeId == null || u.Id != excludeId));
    }
}
