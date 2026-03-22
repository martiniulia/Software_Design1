using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FlowerShop.Repositories;
public class FlowerRepository : IFlowerRepository
{
    private readonly AppDbContext _context;
    public FlowerRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<Flower?> GetByIdAsync(int id)
    {
        return _context.Flowers.FirstOrDefaultAsync(f => f.Id == id);
    }
    public Task<Flower?> GetByIdWithRelationsAsync(int id)
    {
        return _context.Flowers
            .Include(f => f.Category)
            .Include(f => f.Florist)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
    public async Task<List<Flower>> GetAllWithFiltersAsync(string? search, string? sortBy, bool ascending)
    {
        var query = _context.Flowers
            .Include(f => f.Category)
            .Include(f => f.Florist)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(f => f.Name.Contains(search) || (f.Description != null && f.Description.Contains(search)));
        }
        query = sortBy switch
        {
            "price" => ascending ? query.OrderBy(f => f.Price) : query.OrderByDescending(f => f.Price),
            "name" => ascending ? query.OrderBy(f => f.Name) : query.OrderByDescending(f => f.Name),
            "stock" => ascending ? query.OrderBy(f => f.StockQuantity) : query.OrderByDescending(f => f.StockQuantity),
            _ => query.OrderBy(f => f.Name)
        };
        return await query.ToListAsync();
    }
    public Task<List<Category>> GetAllCategoriesAsync()
    {
        return _context.Categories.OrderBy(c => c.Name).ToListAsync();
    }
    public Task<List<Florist>> GetAllFloristsAsync()
    {
        return _context.Florists.OrderBy(f => f.Name).ToListAsync();
    }
    public Task<List<Flower>> GetInStockWithCategoryAsync()
    {
        return _context.Flowers
            .Include(f => f.Category)
            .Where(f => f.StockQuantity > 0)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }
    public async Task AddAsync(Flower flower)
    {
        await _context.Flowers.AddAsync(flower);
    }
    public void Remove(Flower flower)
    {
        _context.Flowers.Remove(flower);
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
