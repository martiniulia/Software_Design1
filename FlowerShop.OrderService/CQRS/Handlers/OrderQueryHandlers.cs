using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.OrderService.CQRS.Queries;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.OrderService.CQRS.Handlers;

public class OrderQueryHandlers :
    IQueryHandler<GetOrderByIdQuery, Order?>,
    IQueryHandler<GetOrdersByUserIdQuery, List<Order>>
{
    private readonly AppDbContext _context;

    public OrderQueryHandlers(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> HandleAsync(GetOrderByIdQuery query)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Flower)
            .FirstOrDefaultAsync(o => o.Id == query.Id);
    }

    public async Task<List<Order>> HandleAsync(GetOrdersByUserIdQuery query)
    {
        return await _context.Orders
            .Where(o => o.UserId == query.UserId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}
