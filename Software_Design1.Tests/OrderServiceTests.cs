using FlowerShop.Models;
using FlowerShop.Repositories.Interfaces;
using FlowerShop.Services;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace FlowerShop.Tests;

public class OrderServiceTests
{
    private static Mock<IDbContextTransaction> CreateTransactionMock()
    {
        var tx = new Mock<IDbContextTransaction>();
        tx.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        tx.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        tx.As<IAsyncDisposable>()
            .Setup(d => d.DisposeAsync())
            .Returns(ValueTask.CompletedTask);
        tx.As<IDisposable>().Setup(d => d.Dispose());
        return tx;
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenCartEmpty_ReturnsFailure()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        orderRepo.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(CreateTransactionMock().Object);
        var sut = new OrderService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);

        var result = await sut.PlaceOrderAsync(1, Array.Empty<CartItem>());

        Assert.False(result.Success);
        Assert.Equal("Cart is empty.", result.ErrorMessage);
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenFlowerMissing_ReturnsFailure()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        orderRepo.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(CreateTransactionMock().Object);
        flowerRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync((Flower?)null);
        var sut = new OrderService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);
        var cart = new[] { new CartItem { Id = 7, Quantity = 1, Price = 5m, IsBouquet = false } };

        var result = await sut.PlaceOrderAsync(1, cart);

        Assert.False(result.Success);
        Assert.Equal("Flower was not found.", result.ErrorMessage);
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenFlowerStockInsufficient_ReturnsFailure()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        orderRepo.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(CreateTransactionMock().Object);
        var rose = new Flower { Id = 2, Name = "Rose", StockQuantity = 1, Price = 3m };
        flowerRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(rose);
        var sut = new OrderService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);
        var cart = new[] { new CartItem { Id = 2, Quantity = 5, Price = 3m, IsBouquet = false } };

        var result = await sut.PlaceOrderAsync(1, cart);

        Assert.False(result.Success);
        Assert.Contains("Insufficient stock", result.ErrorMessage, StringComparison.Ordinal);
        Assert.Contains("Rose", result.ErrorMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenBouquetMissing_ReturnsFailure()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        orderRepo.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(CreateTransactionMock().Object);
        bouquetRepo.Setup(r => r.GetByIdWithFlowersAsync(10)).ReturnsAsync((Bouquet?)null);
        var sut = new OrderService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);
        var cart = new[] { new CartItem { Id = 10, Quantity = 1, Price = 20m, IsBouquet = true } };

        var result = await sut.PlaceOrderAsync(1, cart);

        Assert.False(result.Success);
        Assert.Equal("Bouquet was not found.", result.ErrorMessage);
    }

    [Fact]
    public async Task PlaceOrderAsync_FlowerOnly_DecrementsStockAndCommits()
    {
        var tx = CreateTransactionMock();
        var orderRepo = new Mock<IOrderRepository>();
        var flowerRepo = new Mock<IFlowerRepository>();
        var bouquetRepo = new Mock<IBouquetRepository>();
        orderRepo.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(tx.Object);

        var lily = new Flower { Id = 4, Name = "Lily", StockQuantity = 10, Price = 6m };
        flowerRepo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(lily);

        orderRepo.Setup(r => r.AddOrderAsync(It.IsAny<Order>()))
            .Callback<Order>(o => o.Id = 500)
            .Returns(Task.CompletedTask);
        orderRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        orderRepo.Setup(r => r.AddOrderItemsAsync(It.IsAny<IEnumerable<OrderItem>>())).Returns(Task.CompletedTask);

        var sut = new OrderService(orderRepo.Object, flowerRepo.Object, bouquetRepo.Object);
        var cart = new[] { new CartItem { Id = 4, Quantity = 3, Price = 6m, IsBouquet = false, Name = "Lily" } };

        var result = await sut.PlaceOrderAsync(7, cart);

        Assert.True(result.Success);
        Assert.Equal(7, lily.StockQuantity);
        orderRepo.Verify(r => r.AddOrderAsync(It.Is<Order>(o =>
            o.UserId == 7 && o.Status == OrderStatus.Pending && o.TotalAmount == 18m)), Times.Once);
        orderRepo.Verify(r => r.AddOrderItemsAsync(It.Is<IEnumerable<OrderItem>>(items =>
            items.Single().FlowerId == 4 && items.Single().Quantity == 3 && items.Single().UnitPrice == 6m)), Times.Once);
        tx.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
