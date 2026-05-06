using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FlowerShop.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<bool> EmailExistsAsync(string email)
    {
        return _context.Users.AnyAsync(u => u.Email == email);
    }
    public Task AddAsync(User user)
    {
        return _context.Users.AddAsync(user).AsTask();
    }
    public Task<User?> GetByEmailAndPasswordHashAsync(string email, string passwordHash)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash);
    }
    public Task<User?> GetByIdAsync(int id)
    {
        return _context.Users.FindAsync(id).AsTask();
    }
    public Task<User?> GetByIdWithOrdersAsync(int id)
    {
        return _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<List<User>> GetAllAsync(string? search, UserRole? role)
    {
        var query = _context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.Username.Contains(search) || u.Email.Contains(search));
        }
        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }
        return await query.OrderBy(u => u.Username).ToListAsync();
    }
    public void Remove(User user)
    {
        _context.Users.Remove(user);
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
