using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FlowerShop.Repositories;
public class FloristRepository : IFloristRepository
{
    private readonly AppDbContext _context;
    public FloristRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Florist>> GetAllAsync(string? search)
    {
        var query = _context.Florists.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(f => f.Name.Contains(search)
                                     || (f.Specialization != null && f.Specialization.Contains(search))
                                     || (f.ContactEmail != null && f.ContactEmail.Contains(search)));
        }
        return await query.OrderBy(f => f.Name).ToListAsync();
    }
    public Task<Florist?> GetByIdAsync(int id)
    {
        return _context.Florists.FirstOrDefaultAsync(f => f.Id == id);
    }
    public Task<Florist?> GetByIdWithFlowersAsync(int id)
    {
        return _context.Florists
            .Include(f => f.Flowers)
            .ThenInclude(fl => fl.Category)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
    public Task<Florist?> GetByUserIdAsync(int userId)
    {
        return _context.Florists.FirstOrDefaultAsync(f => f.UserId == userId);
    }
    public Task AddAsync(Florist florist)
    {
        return _context.Florists.AddAsync(florist).AsTask();
    }
    public void Remove(Florist florist)
    {
        _context.Florists.Remove(florist);
    }
    public Task<List<Order>> GetAssignedOrdersByStatusAsync(int floristId, OrderStatus status)
    {
        return _context.Orders
            .Include(o => o.User)
            .Where(o => o.AssignedFloristId == floristId && o.Status == status)
            .OrderBy(o => o.OrderDate)
            .ToListAsync();
    }
    public Task<Order?> GetAssignedOrderDetailsAsync(int orderId)
    {
        return _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Flower)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Bouquet)
            .ThenInclude(b => b!.BouquetFlowers)
            .ThenInclude(bf => bf.Flower)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
