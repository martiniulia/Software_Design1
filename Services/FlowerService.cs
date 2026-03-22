using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class FlowerService : IFlowerService
{
    private readonly IFlowerRepository _flowerRepository;
    public FlowerService(IFlowerRepository flowerRepository)
    {
        _flowerRepository = flowerRepository;
    }
    public Task<List<Flower>> GetIndexDataAsync(string? search, string? sortBy, bool ascending)
    {
        return _flowerRepository.GetAllWithFiltersAsync(search, sortBy, ascending);
    }
    public Task<Flower?> GetDetailsAsync(int id)
    {
        return _flowerRepository.GetByIdWithRelationsAsync(id);
    }
    public Task<List<Category>> GetCategoriesAsync()
    {
        return _flowerRepository.GetAllCategoriesAsync();
    }
    public Task<List<Florist>> GetFloristsAsync()
    {
        return _flowerRepository.GetAllFloristsAsync();
    }
    public async Task CreateAsync(Flower flower)
    {
        await _flowerRepository.AddAsync(flower);
        await _flowerRepository.SaveChangesAsync();
    }
    public async Task<bool> UpdateAsync(int id, Flower flower)
    {
        var existing = await _flowerRepository.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(flower.ImageUrl))
        {
            flower.ImageUrl = existing.ImageUrl;
        }
        existing.Name = flower.Name;
        existing.Description = flower.Description;
        existing.Price = flower.Price;
        existing.StockQuantity = flower.StockQuantity;
        existing.Color = flower.Color;
        existing.CategoryId = flower.CategoryId;
        existing.FloristId = flower.FloristId;
        existing.ImageUrl = flower.ImageUrl;
        await _flowerRepository.SaveChangesAsync();
        return true;
    }
    public async Task DeleteAsync(int id)
    {
        var flower = await _flowerRepository.GetByIdAsync(id);
        if (flower is null)
        {
            return;
        }
        _flowerRepository.Remove(flower);
        await _flowerRepository.SaveChangesAsync();
    }
}
