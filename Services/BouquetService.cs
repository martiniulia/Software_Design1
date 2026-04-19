using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services.Interfaces;
namespace FlowerShop.Services;
public class BouquetService : IBouquetService
{
    private readonly IBouquetRepository _bouquetRepository;
    public BouquetService(IBouquetRepository bouquetRepository)
    {
        _bouquetRepository = bouquetRepository;
    }
    public Task<List<Bouquet>> GetIndexDataAsync(string? search)
    {
        return _bouquetRepository.GetAllWithFiltersAsync(search);
    }
    public Task<Bouquet?> GetDetailsAsync(int id)
    {
        return _bouquetRepository.GetByIdWithDetailsAsync(id);
    }
    public Task<List<Flower>> GetAvailableFlowersAsync()
    {
        return _bouquetRepository.GetAvailableFlowersAsync();
    }
    public async Task<int> CreateAsync(Bouquet bouquet, List<int> flowerIds, List<int> quantities)
    {
        await _bouquetRepository.AddAsync(bouquet);
        await _bouquetRepository.SaveChangesAsync();
        for (var i = 0; i < flowerIds.Count && i < quantities.Count; i++)
        {
            if (quantities[i] <= 0)
            {
                continue;
            }
            await _bouquetRepository.AddBouquetFlowerAsync(new BouquetFlower
            {
                BouquetId = bouquet.Id,
                FlowerId = flowerIds[i],
                Quantity = quantities[i]
            });
        }
        await _bouquetRepository.SaveChangesAsync();
        return bouquet.Id;
    }
    public async Task<bool> UpdateAsync(int id, Bouquet bouquet)
    {
        var existing = await _bouquetRepository.GetByIdWithFlowersAsync(id);
        if (existing is null)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(bouquet.ImageUrl))
        {
            bouquet.ImageUrl = existing.ImageUrl;
        }
        existing.Name = bouquet.Name;
        existing.Price = bouquet.Price;
        existing.Description = bouquet.Description;
        existing.OccasionTag = bouquet.OccasionTag;
        existing.ImageUrl = bouquet.ImageUrl;
        await _bouquetRepository.SaveChangesAsync();
        return true;
    }
    public async Task DeleteAsync(int id)
    {
        var bouquet = await _bouquetRepository.GetByIdWithFlowersAsync(id);
        if (bouquet is null)
        {
            return;
        }
        _bouquetRepository.RemoveBouquetFlowers(bouquet.BouquetFlowers);
        _bouquetRepository.Remove(bouquet);
        await _bouquetRepository.SaveChangesAsync();
    }
}
