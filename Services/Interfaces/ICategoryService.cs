using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface ICategoryService
{
    Task<List<Category>> GetIndexDataAsync(string? search);
    Task<Category?> GetDetailsAsync(int id);
    Task CreateAsync(Category category);
    Task<bool> UpdateAsync(int id, Category category);
    Task<(bool Success, string? ErrorMessage, Category? Category)> DeleteAsync(int id);
}
