using FlowerShop.Models;
namespace FlowerShop.Repositories.Interfaces;
public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(string? search);
    Task<Category?> GetByIdWithFlowersAsync(int id);
    Task<Category?> GetByIdAsync(int id);
    Task AddAsync(Category category);
    void Remove(Category category);
    Task SaveChangesAsync();
}
