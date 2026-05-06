using FlowerShop.Models;
namespace FlowerShop.Repositories.Interfaces;
public interface IFlowerRepository
{
    Task<Flower?> GetByIdAsync(int id);
    Task<Flower?> GetByIdWithRelationsAsync(int id);
    Task<List<Flower>> GetAllWithFiltersAsync(string? search, string? sortBy, bool ascending);
    Task<List<Category>> GetAllCategoriesAsync();
    Task<List<Florist>> GetAllFloristsAsync();
    Task<List<Flower>> GetInStockWithCategoryAsync();
    Task AddAsync(Flower flower);
    void Remove(Flower flower);
    Task SaveChangesAsync();
}
