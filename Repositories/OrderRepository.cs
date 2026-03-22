using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace FlowerShop.Repositories;
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }
    public async Task AddOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }
    public async Task AddOrderItemsAsync(IEnumerable<OrderItem> orderItems)
    {
        await _context.OrderItems.AddRangeAsync(orderItems);
    }
    public async Task<List<Order>> GetOrdersForIndexAsync(bool canManageOrders, int? currentUserId, OrderStatus? status, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.Bouquet)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Flower)
            .AsQueryable();
        if (!canManageOrders)
        {
            query = query.Where(o => o.UserId == currentUserId);
        }
        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }
        if (fromDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= fromDate.Value);
        }
        if (toDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= toDate.Value);
        }
        return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
    }
    public Task<List<Order>> GetHistoryAsync(int userId)
    {
        return _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Flower)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Bouquet)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
    public Task<Order?> GetDetailsAsync(int id)
    {
        return _context.Orders
            .Include(o => o.User)
            .Include(o => o.Bouquet)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Flower)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    public Task<Order?> GetForEditAsync(int id)
    {
        return _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Flower)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    public Task<Order?> GetForDeleteAsync(int id)
    {
        return _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    public void Remove(Order order)
    {
        _context.Orders.Remove(order);
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
