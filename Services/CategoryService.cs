using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public Task<List<Category>> GetIndexDataAsync(string? search)
    {
        return _categoryRepository.GetAllAsync(search);
    }
    public Task<Category?> GetDetailsAsync(int id)
    {
        return _categoryRepository.GetByIdWithFlowersAsync(id);
    }
    public async Task CreateAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }
    public async Task<bool> UpdateAsync(int id, Category category)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }
        existing.Name = category.Name;
        existing.Description = category.Description;
        await _categoryRepository.SaveChangesAsync();
        return true;
    }
    public async Task<(bool Success, string? ErrorMessage, Category? Category)> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdWithFlowersAsync(id);
        if (category is null)
        {
            return (true, null, null);
        }
        if (category.Flowers.Any())
        {
            return (false, "Cannot delete category with associated flowers.", category);
        }
        _categoryRepository.Remove(category);
        await _categoryRepository.SaveChangesAsync();
        return (true, null, null);
    }
}
