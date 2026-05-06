using FlowerShop.Models;
namespace FlowerShop.Repositories.Interfaces;
public interface IBouquetRepository
{
    Task<Bouquet?> GetByIdWithFlowersAsync(int id);
    Task<Bouquet?> GetByIdWithDetailsAsync(int id);
    Task<List<Bouquet>> GetAllWithFiltersAsync(string? search);
    Task<List<Flower>> GetAvailableFlowersAsync();
    Task AddAsync(Bouquet bouquet);
    Task AddBouquetFlowerAsync(BouquetFlower bouquetFlower);
    void RemoveBouquetFlowers(IEnumerable<BouquetFlower> bouquetFlowers);
    void Remove(Bouquet bouquet);
    Task SaveChangesAsync();
}
