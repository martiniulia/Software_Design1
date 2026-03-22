using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface IFlowerService
{
    Task<List<Flower>> GetIndexDataAsync(string? search, string? sortBy, bool ascending);
    Task<Flower?> GetDetailsAsync(int id);
    Task<List<Category>> GetCategoriesAsync();
    Task<List<Florist>> GetFloristsAsync();
    Task CreateAsync(Flower flower);
    Task<bool> UpdateAsync(int id, Flower flower);
    Task DeleteAsync(int id);
}
