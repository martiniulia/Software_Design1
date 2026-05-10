using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FlowerShop.Repositories;
public class BouquetRepository : IBouquetRepository
{
    private readonly AppDbContext _context;
    public BouquetRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<Bouquet?> GetByIdWithFlowersAsync(int id)
    {
        return _context.Bouquets
            .Include(b => b.BouquetFlowers)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
    public Task<Bouquet?> GetByIdWithDetailsAsync(int id)
    {
        return _context.Bouquets
            .Include(b => b.BouquetFlowers)
            .ThenInclude(bf => bf.Flower)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
    public async Task<List<Bouquet>> GetAllWithFiltersAsync(string? search)
    {
        var query = _context.Bouquets
            .Include(b => b.BouquetFlowers)
            .ThenInclude(bf => bf.Flower)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Name.Contains(search) || (b.Description != null && b.Description.Contains(search)));
        }
        return await query.OrderBy(b => b.Name).ToListAsync();
    }
    public Task<List<Flower>> GetAvailableFlowersAsync()
    {
        return _context.Flowers
            .Include(f => f.Category)
            .Where(f => f.StockQuantity > 0)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }
    public async Task AddAsync(Bouquet bouquet)
    {
        await _context.Bouquets.AddAsync(bouquet);
    }
    public async Task AddBouquetFlowerAsync(BouquetFlower bouquetFlower)
    {
        await _context.BouquetFlowers.AddAsync(bouquetFlower);
    }
    public void RemoveBouquetFlowers(IEnumerable<BouquetFlower> bouquetFlowers)
    {
        _context.BouquetFlowers.RemoveRange(bouquetFlowers);
    }
    public void Remove(Bouquet bouquet)
    {
        _context.Bouquets.Remove(bouquet);
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
