using FlowerShop.Models;
namespace FlowerShop.Services.Interfaces;
public interface IFloristsService
{
    Task<List<Florist>> GetIndexAsync(string? search);
    Task<Florist?> GetDetailsAsync(int id);
    Task CreateAsync(Florist florist);
    Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, Florist florist);
    Task<(bool Success, string? ErrorMessage, Florist? Florist)> DeleteAsync(int id);
}
