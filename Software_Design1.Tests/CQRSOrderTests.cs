using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.OrderService.CQRS.Commands;
using FlowerShop.OrderService.CQRS.Handlers;
using FlowerShop.OrderService.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Software_Design1.Tests;

public class CQRSOrderTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrderCommand_ShouldAddOrderToDatabase()
    {
        // Arrange
        var dbContext = GetDbContext();
        var handler = new OrderCommandHandlers(dbContext);
        var command = new CreateOrderCommand
        {
            UserId = 1,
            TotalAmount = 50.5m,
            DeliveryAddress = "123 Main St"
        };

        // Act
        var orderId = await handler.HandleAsync(command);

        // Assert
        var order = await dbContext.Orders.FindAsync(orderId);
        Assert.NotNull(order);
        Assert.Equal(50.5m, order.TotalAmount);
        Assert.Equal("123 Main St", order.DeliveryAddress);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public async Task GetOrdersByUserIdQuery_ShouldReturnUserOrders()
    {
        // Arrange
        var dbContext = GetDbContext();
        dbContext.Orders.Add(new Order { UserId = 1, TotalAmount = 10 });
        dbContext.Orders.Add(new Order { UserId = 1, TotalAmount = 20 });
        dbContext.Orders.Add(new Order { UserId = 2, TotalAmount = 30 });
        await dbContext.SaveChangesAsync();

        var handler = new OrderQueryHandlers(dbContext);
        var query = new GetOrdersByUserIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Equal(2, result.Count);
    }
}
