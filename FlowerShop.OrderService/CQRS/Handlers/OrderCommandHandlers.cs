using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.OrderService.CQRS.Commands;

namespace FlowerShop.OrderService.CQRS.Handlers;

public class OrderCommandHandlers : ICommandHandler<CreateOrderCommand, int>
{
    private readonly AppDbContext _context;

    public OrderCommandHandlers(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> HandleAsync(CreateOrderCommand command)
    {
        var order = new Order
        {
            UserId = command.UserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = command.TotalAmount,
            DeliveryAddress = command.DeliveryAddress ?? string.Empty
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order.Id;
    }
}
