using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FlowerShop.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Category>> GetAllAsync(string? search)
    {
        var query = _context.Categories.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search) || (c.Description != null && c.Description.Contains(search)));
        }
        return await query.OrderBy(c => c.Name).ToListAsync();
    }
    public Task<Category?> GetByIdWithFlowersAsync(int id)
    {
        return _context.Categories
            .Include(c => c.Flowers)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    public Task<Category?> GetByIdAsync(int id)
    {
        return _context.Categories.FindAsync(id).AsTask();
    }
    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }
    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
