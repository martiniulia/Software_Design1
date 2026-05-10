using FlowerShop.Models;
namespace FlowerShop.Repositories.Interfaces;
public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(User user);
    Task<User?> GetByEmailAndPasswordHashAsync(string email, string passwordHash);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByIdWithOrdersAsync(int id);
    Task<List<User>> GetAllAsync(string? search, UserRole? role);
    void Remove(User user);
    Task SaveChangesAsync();
}
