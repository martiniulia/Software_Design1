using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface IBouquetService
{
    Task<List<Bouquet>> GetIndexDataAsync(string? search);
    Task<Bouquet?> GetDetailsAsync(int id);
    Task<List<Flower>> GetAvailableFlowersAsync();
    Task<int> CreateAsync(Bouquet bouquet, List<int> flowerIds, List<int> quantities);
    Task<bool> UpdateAsync(int id, Bouquet bouquet);
    Task DeleteAsync(int id);
}
